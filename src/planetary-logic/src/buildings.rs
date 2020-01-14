use {
    crate::{
        minerals::{Mineral, MiningMineralsComponent},
        DeltaTime, Descriptions,
    },
    serde::Deserialize,
    specs::{
        shred::ResourceId, Component, DenseVecStorage, Entities, Join, NullStorage, Read,
        ReadExpect, ReadStorage, System, SystemData, World, WriteStorage,
    },
    std::{
        hash::Hash,
        sync::{Arc, RwLock},
        time::Duration,
    },
};

#[derive(Deserialize, Copy, Clone, Hash, PartialEq, Eq)]
pub struct BuildingDescId(pub u16);

pub struct BuildTime(pub Duration);

pub struct BuildingDesc {
    pub build_time: Option<BuildTime>,
    pub desc_type: BuildingDescType,
}

pub enum BuildingDescType {
    Mining(MiningBuildingDesc),
}

pub struct MiningBuildingDesc {
    pub minerals: Vec<Mineral>,
}

#[derive(Component)]
#[storage(DenseVecStorage)]
pub struct BuildingComponent {
    pub desc_id: BuildingDescId,
}

#[derive(Component)]
#[storage(DenseVecStorage)]
pub struct BuildComponent {
    pub build_time_elapsed: Duration,
    pub desc_id: BuildingDescId,
}

#[derive(Component, Default)]
#[storage(NullStorage)]
pub struct BuildCompletedComponent;

pub struct BuildTimeSystem;

#[derive(SystemData)]
pub struct BuildTimeSystemData<'a> {
    pub entities: Entities<'a>,

    pub delta_time: Read<'a, DeltaTime>,
    pub descriptions: ReadExpect<'a, Arc<RwLock<Descriptions>>>,

    pub builds: WriteStorage<'a, BuildComponent>,
    pub completed_builds: WriteStorage<'a, BuildCompletedComponent>,
}

impl<'a> System<'a> for BuildTimeSystem {
    type SystemData = BuildTimeSystemData<'a>;

    fn run(&mut self, data: Self::SystemData) {
        let (mut builds, mut completed_builds) = (data.builds, data.completed_builds);

        let read_desc = data.descriptions.read().unwrap();
        let mut completed = Vec::new();
        
        for (entity, build, _) in (&data.entities, &mut builds, !&completed_builds).join() {
            build.build_time_elapsed += data.delta_time.0;

            println!("Build time: {:#?}", &build.build_time_elapsed);

            let desc = read_desc.get_building(&build.desc_id);

            if build.build_time_elapsed >= desc.build_time.as_ref().unwrap().0 {
                completed.push(entity);

                println!("Build completed (ID:{:?})", &build.desc_id.0);
            }
        }
        
        for entity in completed {
            completed_builds.insert(entity, BuildCompletedComponent);

            println!("Build completed component added");
        }
    }
}

pub struct BuildMineSystem;

#[derive(SystemData)]
pub struct BuildMineSystemData<'a> {
    pub entities: Entities<'a>,

    pub descriptions: ReadExpect<'a, Arc<RwLock<Descriptions>>>,

    pub completed_builds: ReadStorage<'a, BuildCompletedComponent>,
    pub builds: WriteStorage<'a, BuildComponent>,
    pub buildings: WriteStorage<'a, BuildingComponent>,
    pub mining_minerals: WriteStorage<'a, MiningMineralsComponent>,
}

impl<'a> System<'a> for BuildMineSystem {
    type SystemData = BuildMineSystemData<'a>;

    fn run(&mut self, data: Self::SystemData) {
        let (mut builds, mut buildings, mut mining_minerals) =
            (data.builds, data.buildings, data.mining_minerals);

        let mut finished_builds = Vec::new();

        for (entity, build, _) in (&data.entities, &mut builds, &data.completed_builds).join() {
            let descriptions = data.descriptions.clone();
            let descriptions = descriptions.read().unwrap();

            let desc = descriptions.get_building(&build.desc_id);
            if let BuildingDescType::Mining(_) = desc.desc_type {
                finished_builds.push((entity, build.desc_id));
            }
        }

        for (entity, desc_id) in finished_builds {
            builds.remove(entity);
            buildings.insert(entity, BuildingComponent { desc_id });
            mining_minerals.insert(
                entity,
                MiningMineralsComponent {
                    minerals: Vec::new(),
                },
            );

            println!("Mine building built: (ID:{:?})", &desc_id.0);
        }
    }
}
