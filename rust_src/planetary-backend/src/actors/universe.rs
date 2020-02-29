use {
    actix::prelude::*,
    planetary_logic::{buildings::BuildingDescId, player::PlayerId, universe::Universe},
    std::time::Duration,
};

pub type BoxedUniverse = Box<Universe<'static, 'static>>;

pub type UniverseAddr = Addr<UniverseActor>;

pub struct UniverseActor {
    pub universe: BoxedUniverse,
}

impl UniverseActor {
    pub fn new(universe: BoxedUniverse) -> Self {
        UniverseActor { universe }
    }
}

impl Actor for UniverseActor {
    type Context = Context<Self>;

    fn started(&mut self, ctx: &mut Self::Context) {
        ctx.run_interval(Duration::from_secs(1), |a, _c| {
            a.universe.step();
        });
    }
}

pub struct BuildMessage {
    pub player_id: PlayerId,
    pub desc_id: BuildingDescId,
}

pub type BuildMessageResult = Result<(), ()>;

impl Message for BuildMessage {
    type Result = BuildMessageResult;
}

impl Handler<BuildMessage> for UniverseActor {
    type Result = BuildMessageResult;

    fn handle(&mut self, msg: BuildMessage, _ctx: &mut Self::Context) -> Self::Result {
        let _entity = self
            .universe
            .create_building_entity(msg.player_id, msg.desc_id);

        Ok(())
    }
}
