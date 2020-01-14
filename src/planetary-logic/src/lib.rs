pub mod buildings;
pub mod minerals;
pub mod planet;
pub mod player;
pub mod universe;

use {
    buildings::{BuildingDesc, BuildingDescId},
    std::{collections::HashMap, time::Duration},
};

pub struct Descriptions {
    pub buildings: HashMap<BuildingDescId, BuildingDesc>,
}

impl Descriptions {
    pub fn get_building(&self, desc_id: &BuildingDescId) -> &BuildingDesc {
        self.buildings
            .get(desc_id)
            .unwrap_or_else(|| panic!("Building desc (ID:{}) not found", desc_id.0))
    }
}

impl Default for Descriptions {
    fn default() -> Self {
        Self {
            buildings: HashMap::new(),
        }
    }
}

#[derive(Default)]
pub struct DeltaTime(pub Duration);
