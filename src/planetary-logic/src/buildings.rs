use {
    crate::minerals::Mineral,
    serde::Deserialize,
    specs::{Component, DenseVecStorage},
    std::hash::Hash,
};

#[derive(Deserialize, Copy, Clone, Hash, PartialEq, Eq)]
pub struct BuildingDescId(pub u16);

pub enum BuildingDesc {
    Mining(Box<MiningBuildingDesc>),
}

pub struct MiningBuildingDesc {
    pub minerals: Vec<Mineral>,
}

#[derive(Component)]
#[storage(DenseVecStorage)]
pub struct BuildingComponent {
    pub desc_id: BuildingDescId,
}
