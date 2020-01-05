use {
    actix::prelude::*,
    planetary_logic::player::PlayerId,
    std::{
        ops::Sub,
        time::{Duration, Instant},
    },
};

pub struct PlayerActor {
    id: PlayerId,
    last_heartbeat: Instant,
}

impl PlayerActor {
    pub fn new(id: PlayerId) -> Self {
        Self {
            id,
            last_heartbeat: Instant::now(),
        }
    }

    pub fn heartbeat(&mut self) {
        self.last_heartbeat = Instant::now();
    }

    fn try_die(&mut self, ctx: &mut Context<Self>) {
        if Instant::now().sub(self.last_heartbeat) >= Duration::from_secs(60) {
            ctx.stop();
        }
    }
}

impl Actor for PlayerActor {
    type Context = Context<Self>;

    fn started(&mut self, ctx: &mut Self::Context) {
        println!("PlayerActor {} started", self.id);

        ctx.run_interval(Duration::from_secs(60), |a, c| a.try_die(c));
    }

    fn stopped(&mut self, _ctx: &mut Self::Context) {
        println!("PlayerActor {} stopped", self.id);
    }
}

pub enum ServiceMessage {
    Heartbeat,
}

pub type PlayerMessageResult = Result<(), ()>;

impl Message for ServiceMessage {
    type Result = PlayerMessageResult;
}

impl Handler<ServiceMessage> for PlayerActor {
    type Result = PlayerMessageResult;

    fn handle(&mut self, msg: ServiceMessage, ctx: &mut Self::Context) -> PlayerMessageResult {
        match msg {
            ServiceMessage::Heartbeat => self.heartbeat(),
        }

        Ok(())
    }
}
