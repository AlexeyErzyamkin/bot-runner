use {
    crate::{
        auth::{self, AuthKey, PlayerWebAuth, UnverifiedPassword, UnverifiedPlayerAccountName},
        storage::Storage,
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
    pub auth_key: AuthKey,
}

pub async fn handle_create(
    (req, data): (web::Json<CreateRequest>, web::Data<Storage>),
) -> Result<HttpResponse, Error> {
    let account_name = match auth::verify_account_name(&req.name) {
        Ok(account_name) => account_name,
        Err(_e) => return Ok(HttpResponse::BadRequest().finish()),
    };

    let password = match auth::verify_password(&req.password) {
        Ok(password) => password,
        Err(_e) => return Ok(HttpResponse::BadRequest().finish()),
    };

    let auth_key = AuthKey(Uuid::new_v4());
    let _web_auth = PlayerWebAuth {
        account_name,
        password,
        auth_key: auth_key.clone(),
        id: PlayerId(1u64),
    };

    let _client = data.client.clone();

    let response = CreateResponse { auth_key };

    Ok(HttpResponse::Ok().json(response))
}

pub async fn handle_login() -> Result<HttpResponse, Error> {
    unimplemented!();
}
