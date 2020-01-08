pub mod buildings;
pub mod minerals;
pub mod planet;
pub mod player;
pub mod universe;

use {
    buildings::{BuildingDesc, BuildingDescId},
    std::{collections::hash_map::HashMap, time::Duration},
};

//pub enum Desc {
//    Building(buildings::BuildingDesc),
//}

pub struct Descriptions {
    pub buildings: HashMap<BuildingDescId, BuildingDesc>,
}

//impl Descriptions {
//    pub fn new() -> Self {
//        Self {
//            buildings: HashMap::new(),
//        }
//    }
//
//    //    pub fn get_building(&self, desc_id: BuildingDescId) -> Option<&BuildingDesc> {
//    //        self.buildings.get(&desc_id)
//    //    }
//}

#[derive(Default)]
pub struct DeltaTime(pub Duration);
