use {
    crate::storage::Storage,
    planetary_logic::player::PlayerId,
    serde::{Deserialize, Serialize},
    std::io,
    uuid::Uuid,
};

#[derive(Deserialize, Serialize, Clone)]
pub struct AuthKey(pub Uuid);

#[derive(Deserialize)]
pub struct UnverifiedPlayerAccountName(pub String);

#[derive(Deserialize)]
pub struct UnverifiedPassword(pub String);

pub struct PlayerAccountName(pub String);

pub struct Password(pub String);

pub fn verify_account_name(
    account_name: &UnverifiedPlayerAccountName,
) -> Result<PlayerAccountName, String> {
    match &account_name.0 {
        s if s.is_empty() => Err("AccountName is empty".to_string()),
        s if s.len() < 3 || s.len() > 12 => {
            Err("AccountName is either too short or too long".to_string())
        }
        s if !s.is_ascii() => Err("AccountName contains non-ASCII chars".to_string()),
        s => Ok(PlayerAccountName(s.clone())),
    }
}

pub fn verify_password(password: &UnverifiedPassword) -> Result<Password, String> {
    match &password.0 {
        p if p.is_empty() => Err("Password is empty".to_string()),
        p if p.len() > 50 => Err("Password too long".to_string()),
        p => Ok(Password(p.clone())),
    }
}

pub struct NewPlayerWebAuth {
    pub auth_key: AuthKey,
    pub account_name: PlayerAccountName,
    pub password: Password,
}

pub struct PlayerWebAuth {
    pub id: PlayerId,
    pub auth_key: AuthKey,
    pub account_name: PlayerAccountName,
    pub password: Password,
}

impl Storage {
    pub async fn insert_player_web_auth(&self, auth: &NewPlayerWebAuth) -> io::Result<PlayerId> {
        let client = self.client.clone();

        let auth_key_str = auth.auth_key.0.to_string();
        let result = client
            .query(
                "insert into auth_web (player_id, auth_key, account_name, password) values (default, $1, $2, $3) returning player_id",
                &[&auth_key_str, &auth.account_name.0, &auth.password.0]
            ).await;

        match result {
            Ok(rows) => {
                let id = rows.iter().next().unwrap().get(0);

                Ok(PlayerId(id))
            }
            Err(e) => {
                eprintln!("Error inserting PlayerWebAuth: {}", e);

                match e {
                    _ => Err(io::ErrorKind::Other.into()),
                }
            }
        }
    }
}
