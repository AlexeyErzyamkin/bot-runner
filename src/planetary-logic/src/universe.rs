use {
    crate::{
        buildings::{BuildingComponent, BuildingDescId},
        minerals::{
            Amount, ContainMineralsComponent, Mineral, MiningMineralsComponent,
            MiningMineralsSystem,
        },
        planet::{OnPlanetComponent, PlanetComponent, PlanetId},
        player::{OwnByPlayerComponent, PlayerId},
        DeltaTime, Descriptions,
    },
    specs::{world::Builder, Dispatcher, DispatcherBuilder, Entity, World, WorldExt},
    std::{
        sync::{Arc, RwLock},
        time::Duration,
    },
};

pub struct Universe<'a, 'b> {
    pub world: World,
    pub dispatcher: Dispatcher<'a, 'b>,
}

impl<'a, 'b> Universe<'a, 'b> {
    pub fn new(descriptions: Arc<RwLock<Descriptions>>) -> Self {
        let mut world = World::new();
        let mut dispatcher = DispatcherBuilder::new()
            .with(MiningMineralsSystem, "mining", &[])
            .build();

        // Components
        world.register::<PlanetComponent>();
        world.register::<OnPlanetComponent>();
        world.register::<ContainMineralsComponent>();
        world.register::<MiningMineralsComponent>();
        world.register::<BuildingComponent>();
        world.register::<OwnByPlayerComponent>();

        // Resources
        world.insert(descriptions);
        world.insert(DeltaTime(Duration::from_secs(1)));

        dispatcher.setup(&mut world);

        world
            .create_entity()
            .with(PlanetComponent {
                planet_id: PlanetId(1),
            })
            .with(ContainMineralsComponent {
                minerals: vec![Mineral::Iron(Amount(1000u64))],
            })
            .build();

        Self { world, dispatcher }
    }

    pub fn step(&mut self) {
        self.dispatcher.dispatch(&mut self.world);
        self.world.maintain();
    }

    pub fn create_building_entity(
        &mut self,
        player_id: PlayerId,
        desc_id: BuildingDescId,
    ) -> Entity {
        println!("Building entity: {}", desc_id.0);

        self.world
            .create_entity()
            .with(OwnByPlayerComponent { player_id })
            .with(BuildingComponent { desc_id })
            .with(MiningMineralsComponent {
                minerals: vec![Mineral::Iron(Amount(10))],
            })
            .build()
    }
}
