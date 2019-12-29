use {
    std::{io, sync::Arc},
    tokio,
    tokio_postgres::{self, Client, NoTls},
};

#[derive(Clone)]
pub struct Storage {
    pub client: Arc<Client>,
}

pub async fn init(config: &str) -> io::Result<Storage> {
    let (client, connection) =
        tokio_postgres::connect(config, NoTls)
            .await
            .map_err(|e| -> io::Error {
                eprintln!("Error connecting to PostgreSQL: {}", e);

                io::ErrorKind::Other.into()
            })?;

    tokio::spawn(async move {
        if let Err(e) = connection.await {
            eprintln!("PostgreSQL connection failed: {}", e);
        }
    });

    Ok(Storage {
        client: Arc::new(client),
    })
}
