use {
    crate::{storage::Storage, Error, Result},
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
) -> Result<PlayerAccountName> {
    match &account_name.0 {
        s if s.is_empty() => Err(Error::Validation("AccountName is empty".to_string())),
        s if s.len() < 3 || s.len() > 12 => Err(Error::Validation(
            "AccountName is either too short or too long".to_string(),
        )),
        s if !s.is_ascii() => Err(Error::Validation(
            "AccountName contains non-ASCII chars".to_string(),
        )),
        s => Ok(PlayerAccountName(s.clone())),
    }
}

pub fn verify_password(password: &UnverifiedPassword) -> Result<Password> {
    match &password.0 {
        p if p.is_empty() => Err(Error::Validation("Password is empty".to_string())),
        p if p.len() > 50 => Err(Error::Validation("Password too long".to_string())),
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
    pub async fn insert_player_web_auth(&self, auth: &NewPlayerWebAuth) -> Result<PlayerId> {
        let auth_key_str = auth.auth_key.0.to_string();
        let rows = self.client
            .query(
                "insert into auth_web (player_id, auth_key, account_name, password) values (default, $1, $2, $3) returning player_id",
                &[&auth_key_str, &auth.account_name.0, &auth.password.0]
            ).await?;

        let id = rows
            .iter()
            .next()
            .ok_or_else(|| -> io::Error { io::ErrorKind::NotFound.into() })?
            .get(0);

        Ok(PlayerId(id))
    }

    pub async fn get_player_web_auth(
        &self,
        account_name: &PlayerAccountName,
        password: &Password,
    ) -> Result<Option<PlayerWebAuth>> {
        let rows = self.client
            .query(
                "select player_id, auth_key from auth_web where account_name = $1 and password = $2",
                &[&account_name.0, &password.0]
            ).await?;

        if rows.is_empty() {
            Ok(None)
        } else {
            let result = rows.iter().next().map(|row| {
                let pid: i64 = row.get(0);
                let ak: String = row.get(1);

                PlayerWebAuth {
                    id: PlayerId(pid),
                    auth_key: AuthKey(Uuid::parse_str(&ak).unwrap()),
                    account_name: PlayerAccountName(account_name.0.to_owned()),
                    password: Password(password.0.to_owned()),
                }
            });

            Ok(result)
        }
    }
}
