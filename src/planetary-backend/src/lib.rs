pub mod auth;
pub mod error;
pub mod player;
pub mod storage;

pub use error::{Error, ErrorResponse};

pub type Result<T, E = Error> = ::std::result::Result<T, E>;
