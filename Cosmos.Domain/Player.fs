namespace Cosmos.Domain

type PlayerName = PlayerName of string

//module PlayerName =

//    type T = PlayerName of string

//    let createWithCont success failure (s:string) =
//        if s.Length < 3 || s.Length > 20
//            then success (PlayerName s)
//            else failure "PlayerName invalid"

//    let create s =
//        let success p = Some p
//        let failure _ = None
//        createWithCont success failure s

//    let value (PlayerName p) = p

module Player =

    type T =
        {
            Name: PlayerName
        }