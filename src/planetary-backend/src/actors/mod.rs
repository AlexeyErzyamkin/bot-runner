use {
    actix::prelude::*,
    std::{
        collections::HashMap,
        hash::Hash,
        sync::{Arc, RwLock},
    },
};

pub mod player;

pub struct ActorsDispatcher<T, I>
where
    T: Actor,
    I: Hash + Eq + Copy,
{
    addrs: Arc<RwLock<HashMap<I, Addr<T>>>>,
}

impl<T, I> Default for ActorsDispatcher<T, I>
where
    T: Actor,
    I: Hash + Eq + Copy,
{
    fn default() -> Self {
        Self {
            addrs: Arc::new(RwLock::new(HashMap::new())),
        }
    }
}

impl<T, I> ActorsDispatcher<T, I>
where
    T: Actor,
    I: Hash + Eq + Copy,
{
    //    pub fn new() -> Self {
    //        Self {
    //            addrs: Arc::new(RwLock::new(HashMap::new())),
    //        }
    //    }

    pub fn start<F>(&self, id: I, f: F)
    where
        F: Fn(I) -> Addr<T> + 'static,
    {
        let addr = f(id);

        let locked_addrs = self.addrs.clone();
        let mut write_addrs = locked_addrs.write().unwrap();

        write_addrs.insert(id, addr);
    }

    pub fn get_addr(&self, id: &I) -> Option<Addr<T>> {
        let locked_addrs = self.addrs.clone();
        let read_addrs = locked_addrs.read().unwrap();

        read_addrs.get(id).map(|addr| addr.to_owned())
    }
}
