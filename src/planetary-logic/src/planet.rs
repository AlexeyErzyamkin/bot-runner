use specs::{Component, DenseVecStorage};

pub struct PlanetId(pub u32);

#[derive(Component)]
#[storage(DenseVecStorage)]
pub struct PlanetComponent {
    pub planet_id: PlanetId,
}

#[derive(Component)]
#[storage(DenseVecStorage)]
pub struct OnPlanetComponent {
    pub planet_id: PlanetId,
}
