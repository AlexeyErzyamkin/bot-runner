use {
    serde::{Deserialize, Serialize},
    std::{
        fmt::{self, Display, Formatter},
        hash::Hash,
    },
};

#[derive(Serialize, Deserialize, Hash, PartialEq, Eq, Copy, Clone)]
pub struct PlayerId(pub i64);

impl Display for PlayerId {
    fn fmt(&self, f: &mut Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)?;

        Ok(())
    }
}

pub struct PlayerVersion(pub i16);

pub struct PlayerName(pub String);

pub struct Player {
    pub id: PlayerId,
    pub version: PlayerVersion,
    pub name: PlayerName,
}
