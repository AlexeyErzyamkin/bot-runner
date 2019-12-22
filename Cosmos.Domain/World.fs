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
        let rand = System.Random()

        let (Width maxX) = w
        let (Height maxY) = h
        let maxX = int maxX
        let maxY = int maxY

        let coords =
            seq {
                for _ in 1..planetsCount do
                    (X (uint32 (rand.Next maxX)), Y (uint32 (rand.Next maxY)))
            }

        let createPlanet (x, y) = {X = x; Y = y}

        {
            Width = w;
            Height = h;
            Planets =
                coords
                |> Seq.map createPlanet
                |> Seq.toList
        }

    type WorldId = WorldId of string

    type T =
        {
            Id: WorldId;
            Universe: Universe;
        }