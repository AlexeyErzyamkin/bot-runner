use serde::Serialize;

#[derive(Serialize)]
pub struct PlayerId(pub i64);

pub struct PlayerVersion(pub i16);

pub struct PlayerName(pub String);

pub struct Player {
    pub id: PlayerId,
    pub version: PlayerVersion,
    pub name: PlayerName,
}
