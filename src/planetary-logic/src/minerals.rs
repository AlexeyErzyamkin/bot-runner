use {
    crate::{
        buildings::{BuildingComponent, BuildingDesc, BuildingDescType, MiningBuildingDesc},
        DeltaTime, Descriptions,
    },
    specs::{
        shred::ResourceId, Component, DenseVecStorage, Join, Read, ReadExpect, ReadStorage, System,
        SystemData, World, WriteStorage,
    },
    std::{
        sync::{Arc, RwLock},
        time::Duration,
    },
};

#[derive(Debug, Default)]
pub struct Amount(pub u64);

#[derive(Debug)]
pub enum Mineral {
    Iron(Amount),
}

//#[derive(Component, Debug)]
//#[storage(DenseVecStorage)]
//pub struct ContainMineralsComponent {
//    pub minerals: Vec<Mineral>,
//}

/*
Идём по планетам, ищем добывалки с этих планет, маркируем их через FlaggedStorage,
списываем ресурсы и добавляем им
*/

pub struct MiningMineralsInterval(pub Duration);

#[derive(Component, Debug)]
#[storage(DenseVecStorage)]
pub struct MiningMineralsComponent {
    pub mined_amount: Amount,
}

pub struct MiningMineralsSystem;

#[derive(SystemData)]
pub struct MiningMineralsSystemData<'a> {
    pub descriptions: ReadExpect<'a, Arc<RwLock<Descriptions>>>,

    pub delta_time: Read<'a, DeltaTime>,

    pub buildings: ReadStorage<'a, BuildingComponent>,
    pub mines: WriteStorage<'a, MiningMineralsComponent>,
}

impl<'a> System<'a> for MiningMineralsSystem {
    type SystemData = (
        ReadExpect<'a, Arc<RwLock<Descriptions>>>,
        Read<'a, DeltaTime>,
        ReadStorage<'a, BuildingComponent>,
        WriteStorage<'a, MiningMineralsComponent>,
    );

    fn run(
        &mut self,
        (descriptions, delta_time, buildings, mut mining_minerals): Self::SystemData,
    ) {
        let descriptions = descriptions.clone();
        let descriptions = descriptions.read().unwrap();

        for (building, mining_mineral) in (&buildings, &mut mining_minerals).join() {
            let desc = descriptions.get_building(&building.desc_id);

            if let BuildingDescType::Mining(mining_desc) = &desc.desc_type {
                if let Mineral::Iron(amount) = &mining_desc.mineral {
                    mining_mineral.mined_amount.0 += amount.0;

                    println!(
                        "Mined {} iron. New value: {}",
                        amount.0, mining_mineral.mined_amount.0
                    );
                }
            }
        }
    }
}

/* Шахта
    ПотребляюЭнергиюКомпонент
    ДобываюПолезныеИскопаемыеКомпонент
*/
