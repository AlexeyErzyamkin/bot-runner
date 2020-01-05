use specs::prelude::*;

pub struct Universe {
    pub world: World,
}

impl Universe {
    pub fn new() -> Self {
        Self {
            world: World::new(),
        }
    }

    pub fn create_player(&mut self) -> Entity {
        self.world.create_entity().build()
    }
}
