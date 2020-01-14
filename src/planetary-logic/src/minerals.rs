use {
    crate::{
        buildings::{BuildingComponent, BuildingDesc, MiningBuildingDesc},
        DeltaTime, Descriptions,
    },
    specs::{
        shred::ResourceId, Component, DenseVecStorage, Join, Read, ReadExpect, ReadStorage, System,
        SystemData, World, WriteStorage,
    },
    std::{
        marker::PhantomData,
        sync::{Arc, RwLock},
        time::Duration,
    },
};

pub trait Mineral: Send + Sync + 'static {}

#[derive(Debug)]
pub struct IronMineral;

impl Mineral for IronMineral {}

#[derive(Debug, Clone)]
pub struct Amount<T>(pub f64, PhantomData<T>);

impl<T> Amount<T> {
    pub fn new(value: f64) -> Self {
        Self(value, PhantomData)
    }
}

impl<T> Default for Amount<T> {
    fn default() -> Self {
        Self::new(0f64)
    }
}

pub enum MineralAmount {
    Iron(Amount<IronMineral>),
}

#[derive(Component, Debug)]
#[storage(DenseVecStorage)]
pub struct ContainMineralComponent<T: Mineral> {
    pub amount: Amount<T>,
    _p: PhantomData<T>,
}

impl<T: Mineral> ContainMineralComponent<T> {
    pub fn new(amount: Amount<T>) -> Self {
        Self {
            amount,
            _p: PhantomData,
        }
    }
}

/*
Идём по планетам, ищем добывалки с этих планет, маркируем их через FlaggedStorage,
списываем ресурсы и добавляем им
*/

pub struct MiningMineralInterval(pub Duration);

#[derive(Component, Debug)]
#[storage(DenseVecStorage)]
pub struct MiningMineralComponent<T: Mineral> {
    pub mine_minerals_amount: Amount<T>,
    pub mined_minerals: Amount<T>,
}

impl<T: Mineral> MiningMineralComponent<T> {
    pub fn new(mine_minerals_amount: Amount<T>) -> Self {
        Self {
            mine_minerals_amount,
            mined_minerals: Amount::default(),
        }
    }
}

pub struct MiningMineralSystem<T: Mineral> {
    _p: PhantomData<T>,
}

impl<T: Mineral> Default for MiningMineralSystem<T> {
    fn default() -> Self {
        Self { _p: PhantomData }
    }
}

#[derive(SystemData)]
pub struct MiningMineralSystemData<'a, T: Mineral> {
    pub descriptions: ReadExpect<'a, Arc<RwLock<Descriptions>>>,

    pub delta_time: Read<'a, DeltaTime>,

    pub buildings: ReadStorage<'a, BuildingComponent>,
    pub mines: WriteStorage<'a, MiningMineralComponent<T>>,
}

impl<'a, T: Mineral> System<'a> for MiningMineralSystem<T> {
    type SystemData = MiningMineralSystemData<'a, T>;

    fn run(&mut self, data: Self::SystemData) {
        let mut mines = data.mines;

        let descriptions = data.descriptions.clone();
        let descriptions = descriptions.read().unwrap();

        for (building, mine) in (&data.buildings, &mut mines).join() {
            let desc = descriptions.get_building(&building.desc_id);

            //                        if let BuildingDesc::Mining(mining_desc) = desc {
        }

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
