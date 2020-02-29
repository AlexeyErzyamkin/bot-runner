use {
    crate::{
        actors::{
            player::{BuildMessage, PlayerActor, ServiceMessage},
            universe::UniverseAddr,
            ActorsDispatcher,
        },
        auth::{self, AuthKey, NewPlayerWebAuth, UnverifiedPassword, UnverifiedPlayerAccountName},
        storage::Storage,
        Error, ErrorResponse, Result,
    },
    actix::Actor,
    actix_web::{web, HttpResponse},
    planetary_logic::{buildings::BuildingDescId, player::PlayerId},
    serde::{Deserialize, Serialize},
    std::sync::Arc,
    uuid::Uuid,
};

#[derive(Clone)]
pub struct RequestContext {
    pub storage: Arc<Storage>,
    pub actors_dispatcher: Arc<ActorsDispatcher<PlayerActor, PlayerId>>,
    pub universe_addr: UniverseAddr,
}

impl RequestContext {
    pub fn new(storage: Storage, universe_addr: UniverseAddr) -> Self {
        RequestContext {
            storage: Arc::new(storage),
            actors_dispatcher: Arc::new(ActorsDispatcher::default()),
            universe_addr,
        }
    }
}

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
    (req, data): (web::Json<CreateRequest>, web::Data<RequestContext>),
) -> Result<HttpResponse> {
    let account_name = auth::verify_account_name(&req.name)?;
    let password = auth::verify_password(&req.password)?;

    let auth_key = AuthKey(Uuid::new_v4());
    let web_auth = NewPlayerWebAuth {
        account_name,
        password,
        auth_key: auth_key.clone(),
    };

    let player_id = data.storage.insert_player_web_auth(&web_auth).await?;

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
    (req, data): (web::Json<LoginRequest>, web::Data<RequestContext>),
) -> Result<HttpResponse> {
    let account_name = auth::verify_account_name(&req.account_name)?;
    let password = auth::verify_password(&req.password)?;

    let universe_addr = data.universe_addr.clone();

    match data
        .storage
        .get_player_web_auth(&account_name, &password)
        .await?
    {
        Some(player_auth) => {
            data.actors_dispatcher.start(player_auth.id, move |id| {
                PlayerActor::new(id, universe_addr.clone()).start()
            });

            Ok(HttpResponse::Ok().json(LoginResponse {
                player_id: player_auth.id,
                auth_key: player_auth.auth_key,
            }))
        }
        None => Ok(HttpResponse::NotFound().json(ErrorResponse {
            error: "Player not found".to_string(),
        })),
    }
}

#[derive(Deserialize)]
pub struct HeartbeatRequest {
    pub player_id: PlayerId,
    pub auth_key: AuthKey,
}

pub async fn handle_heartbeat(
    (req, data): (web::Json<HeartbeatRequest>, web::Data<RequestContext>),
) -> Result<HttpResponse> {
    match data.actors_dispatcher.get_addr(&req.player_id) {
        Some(ref addr) => {
            addr.send(ServiceMessage::Heartbeat)
                .await?
                .expect("Must always succeed");

            Ok(HttpResponse::Ok().finish())
        }
        None => Err(Error::Validation("Player not found".to_string())),
    }
}

#[derive(Deserialize)]
pub struct BuildRequest {
    pub player_id: PlayerId,
    pub auth_key: AuthKey,
    pub desc_id: BuildingDescId,
}

pub async fn handle_build(
    (req, data): (web::Json<BuildRequest>, web::Data<RequestContext>),
) -> Result<HttpResponse> {
    let addr = &data
        .actors_dispatcher
        .get_addr(&req.player_id)
        .ok_or_else(|| Error::Validation("Player not found".to_string()))?;

    let response = addr
        .send(BuildMessage {
            desc_id: req.desc_id,
        })
        .await?;

    match response {
        Ok(_) => Ok(HttpResponse::Ok().finish()),
        Err(_) => Err(Error::Validation("Huynya".to_string())),
    }
}
