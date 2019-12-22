namespace Cosmos.Domain

module World =

    type Width = Width of uint32
    type Height = Height of uint32

    type X = X of uint32
    type Y = Y of uint32

    type Planet =
        {
        X: X;
        Y: Y;
        }

    type Universe =
        {
        Width: Width;
        Height: Height;
        Planets: Planet list;
        }

    let create (w:Width) (h:Height) planetsCount =
        let rand = new System.Random()
        let nextRandom max =
            uint32 (rand.Next (0, int (max)))

        let coords =
            let (Width maxX) = w
            let (Height maxY) = h
            seq {
                for _ in 1..planetsCount do
                    (X (nextRandom maxX), Y (nextRandom maxY))
            }

        let createPlanet (x : X, y : Y) = {X = x; Y = y}

        {
        Width=w;
        Height=h;
        Planets =
            coords
            |> Seq.map createPlanet
            |> Seq.toList
        }