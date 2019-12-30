use {
    crate::{
        auth::{
            self, AuthKey, NewPlayerWebAuth, PlayerWebAuth, UnverifiedPassword,
            UnverifiedPlayerAccountName,
        },
        storage::Storage,
        ErrorResponse,
    },
    actix_web::{web, Error, HttpResponse},
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
) -> Result<HttpResponse, Error> {
    let account_name = match auth::verify_account_name(&req.name) {
        Ok(account_name) => account_name,
        Err(error) => return Ok(HttpResponse::BadRequest().json(ErrorResponse { error })),
    };

    let password = match auth::verify_password(&req.password) {
        Ok(password) => password,
        Err(error) => return Ok(HttpResponse::BadRequest().json(ErrorResponse { error })),
    };

    let auth_key = AuthKey(Uuid::new_v4());
    let web_auth = NewPlayerWebAuth {
        account_name,
        password,
        auth_key: auth_key.clone(),
    };

    let _client = data.client.clone();

    match data.insert_player_web_auth(&web_auth).await {
        Ok(player_id) => Ok(HttpResponse::Ok().json(CreateResponse {
            player_id,
            auth_key,
        })),
        Err(_e) => Ok(HttpResponse::InternalServerError().json(ErrorResponse {
            error: "DB error".to_string(),
        })),
    }
}

pub async fn handle_login() -> Result<HttpResponse, Error> {
    unimplemented!();
}
