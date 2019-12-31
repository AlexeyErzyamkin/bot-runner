use {
    actix_web::{HttpResponse, ResponseError},
    serde::{export::Formatter, Serialize},
    std::fmt::{self, Display},
};

#[derive(Serialize)]
pub struct ErrorResponse {
    pub error: String,
}

#[derive(Debug)]
pub enum Error {
    Validation(String),
    Configuration,
    Db(::tokio_postgres::Error),
    Io(::std::io::Error),
}

impl ResponseError for Error {
    fn error_response(&self) -> HttpResponse {
        match self {
            Error::Validation(e) => HttpResponse::BadRequest().json(ErrorResponse {
                error: format!("Validation error: {}", e),
            }),
            Error::Configuration => HttpResponse::InternalServerError().json(ErrorResponse {
                error: "Invalid configuration".to_string(),
            }),
            Error::Db(e) => HttpResponse::InternalServerError().json(ErrorResponse {
                error: format!("DB error: {}", e),
            }),
            Error::Io(e) => HttpResponse::InternalServerError().json(ErrorResponse {
                error: format!("IO error: {}", e),
            }),
        }
    }
}

impl Display for Error {
    fn fmt(&self, f: &mut Formatter<'_>) -> fmt::Result {
        match self {
            Error::Validation(e) => write!(f, "Validation error: {}", e)?,
            Error::Configuration => f.write_str("Configuration error")?,
            Error::Db(e) => write!(f, "DB error: {}", e)?,
            Error::Io(e) => write!(f, "IO error: {}", e)?,
        };

        Ok(())
    }
}

impl From<::std::io::Error> for Error {
    fn from(e: ::std::io::Error) -> Self {
        Error::Io(e)
    }
}

impl From<::tokio_postgres::Error> for Error {
    fn from(e: ::tokio_postgres::Error) -> Self {
        Error::Db(e)
    }
}

impl From<::std::env::VarError> for Error {
    fn from(_: ::std::env::VarError) -> Self {
        Error::Configuration
    }
}
