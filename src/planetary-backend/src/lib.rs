pub mod auth;
pub mod player;
pub mod storage;

use serde::Serialize;

#[derive(Serialize)]
pub struct ErrorResponse {
    pub error: String,
}
