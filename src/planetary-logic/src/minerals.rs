use {
    crate::{
        buildings::{BuildingComponent, BuildingDesc, MiningBuildingDesc},
        DeltaTime, Descriptions,
    },
    specs::{
        shred::ResourceId, Component, DenseVecStorage, Join, Read, ReadExpect, ReadStorage, System,
        SystemData, World, WriteStorage,
    },
    std::{time::Duration,sync::{Arc, RwLock}},
};

#[derive(Debug)]
pub struct Amount(pub u64);

#[derive(Debug)]
pub enum Mineral {
    Iron(Amount),
}

#[derive(Component, Debug)]
#[storage(DenseVecStorage)]
pub struct ContainMineralsComponent {
    pub minerals: Vec<Mineral>,
}

/*
Идём по планетам, ищем добывалки с этих планет, маркируем их через FlaggedStorage,
списываем ресурсы и добавляем им
*/

pub struct MiningMineralsInterval(pub Duration);

#[derive(Component, Debug)]
#[storage(DenseVecStorage)]
pub struct MiningMineralsComponent {
    pub minerals: Vec<Mineral>,
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
        //        let descriptions = descriptions.clone();
        //        let descriptions = descriptions.read().unwrap();
        //
        //        for (building, mining_mineral) in (&buildings, &mut mining_minerals).join() {
        //            let desc = descriptions.get_building(&building.desc_id);
        //
        //            if let BuildingDesc::Mining(mining_desc) = desc {
        //                for desc_mineral in mining_desc.minerals.iter() {
        //                    match desc_mineral {
        //                        Mineral::Iron(desc_amount) => {
        //                            for mined_mineral in mining_mineral.minerals.iter_mut() {
        //                                match mined_mineral {
        //                                    Mineral::Iron(ref mut mined_amount) => {
        //                                        //                                        delta_time.0.
        //                                        mined_amount.0 += desc_amount.0;
        //
        //                                        println!(
        //                                            "Mined {} iron. New value: {}",
        //                                            desc_amount.0, mined_amount.0
        //                                        );
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
    }
}

/* Шахта
    ПотребляюЭнергиюКомпонент
    ДобываюПолезныеИскопаемыеКомпонент
*/
