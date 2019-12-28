pub struct PlayerId(pub u64);

pub struct PlayerVersion(pub u64);

pub struct PlayerName(pub String);

pub struct Player {
    pub id: PlayerId,
    pub version: PlayerVersion,
    pub name: PlayerName,
}
