use {
    crate::{
        buildings::{
            BuildCompletedComponent, BuildComponent, BuildMineSystem, BuildTimeSystem,
            BuildingComponent, BuildingDesc, BuildingDescId,
        },
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
        ops::Sub,
        sync::{Arc, RwLock},
        time::{Duration, Instant},
    },
};

pub struct Universe<'a, 'b> {
    pub world: World,
    pub dispatcher: Dispatcher<'a, 'b>,
    last_step_time: Instant,
    descriptions: Arc<RwLock<Descriptions>>,
}

impl<'a, 'b> Universe<'a, 'b> {
    pub fn new(descriptions: Arc<RwLock<Descriptions>>) -> Self {
        let mut world = World::new();
        let mut dispatcher = DispatcherBuilder::new()
            .with(MiningMineralsSystem, "mining", &[])
            .with(BuildTimeSystem, "build_time", &[])
            .with(BuildMineSystem, "build_mine", &["build_time"])
            .build();

        // Components
        world.register::<PlanetComponent>();
        world.register::<OnPlanetComponent>();
        world.register::<ContainMineralsComponent>();
        world.register::<MiningMineralsComponent>();
        world.register::<BuildingComponent>();
        world.register::<BuildComponent>();
        world.register::<BuildCompletedComponent>();
        world.register::<OwnByPlayerComponent>();

        // Resources
        world.insert(descriptions.clone());
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

        Self {
            world,
            dispatcher,
            last_step_time: Instant::now(),
            descriptions: descriptions.clone(),
        }
    }

    pub fn step(&mut self) {
        let now = Instant::now();
        let elapsed_time = now.sub(self.last_step_time);

//        println!("Step elapsed time: {:#?}", &elapsed_time);

        let mut world = &mut self.world;

        {
            let mut delta_time_resource = world.write_resource::<DeltaTime>();
            *delta_time_resource = DeltaTime(elapsed_time);
        }

        self.dispatcher.dispatch(world);
        world.maintain();

        self.last_step_time = now;
    }

    pub fn create_building_entity(
        &mut self,
        player_id: PlayerId,
        desc_id: BuildingDescId,
    ) -> Entity {
        println!("Building entity: {}", desc_id.0);

        let mut builder = self
            .world
            .create_entity()
            .with(OwnByPlayerComponent { player_id });

        let has_build_time = {
            let desc_read = self.descriptions.read().unwrap();
            let building_desc = desc_read.get_building(&desc_id);

            building_desc.build_time.is_some()
        };

        builder = if has_build_time {
            builder.with(BuildComponent {
                build_time_elapsed: Duration::default(),
                desc_id,
            })
        } else {
            builder
                .with(BuildingComponent { desc_id })
                .with(MiningMineralsComponent {
                    minerals: Vec::new(),
                })
        };

        builder.build()
    }
}
