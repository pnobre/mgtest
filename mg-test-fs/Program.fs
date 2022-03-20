open Game

[<EntryPoint>]
let main argv =
    use g = new Game1 ()
    do g.Run ()
    0 
