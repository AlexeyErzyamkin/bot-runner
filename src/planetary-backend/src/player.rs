use {
    crate::{
        auth::{self, AuthKey, NewPlayerWebAuth, UnverifiedPassword, UnverifiedPlayerAccountName},
        storage::Storage,
        ErrorResponse, Result,
    },
    actix_web::{web, HttpResponse},
    planetary_logic::player::PlayerId,
    serde::{Deserialize, Serialize},
    uuid::Uuid,
};

#[derive(Deserialize)]
pub struct CreateRequest {
    pub name: UnverifiedPlayerAccountName,
    pub password: UnverifiedPassword,
}

#[derive(Serialize)]
pub struct CreateResponse {
    pub player_id: PlayerId,
    pub auth_key: AuthKey,
}

pub async fn handle_create(
    (req, data): (web::Json<CreateRequest>, web::Data<Storage>),
) -> Result<HttpResponse> {
    let account_name = auth::verify_account_name(&req.name)?;
    let password = auth::verify_password(&req.password)?;

    let auth_key = AuthKey(Uuid::new_v4());
    let web_auth = NewPlayerWebAuth {
        account_name,
        password,
        auth_key: auth_key.clone(),
    };

    let player_id = data.insert_player_web_auth(&web_auth).await?;

    Ok(HttpResponse::Ok().json(CreateResponse {
        player_id,
        auth_key,
    }))
}

#[derive(Deserialize)]
pub struct LoginRequest {
    pub account_name: UnverifiedPlayerAccountName,
    pub password: UnverifiedPassword,
}

#[derive(Serialize)]
pub struct LoginResponse {
    pub player_id: PlayerId,
    pub auth_key: AuthKey,
}

pub async fn handle_login(
    (req, data): (web::Json<LoginRequest>, web::Data<Storage>),
) -> Result<HttpResponse> {
    let account_name = auth::verify_account_name(&req.account_name)?;
    let password = auth::verify_password(&req.password)?;

    match data.get_player_web_auth(&account_name, &password).await? {
        Some(player_auth) => Ok(HttpResponse::Ok().json(LoginResponse {
            player_id: player_auth.id,
            auth_key: player_auth.auth_key,
        })),
        None => Ok(HttpResponse::NotFound().json(ErrorResponse {
            error: "Player not found".to_string(),
        })),
    }
}
