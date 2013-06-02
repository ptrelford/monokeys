namespace MonoKeys

open System
open System.Windows
open System.Windows.Controls
open Microsoft.Xna.Framework.Audio

type App() as this = 
    inherit Application()

    let sampleRate = 8000

    let sample x = x * 32767. |> int16

    let toBytes (xs:int16[]) =
        let bytes = Array.CreateInstance(typeof<byte>, 2 * xs.Length)
        Buffer.BlockCopy(xs, 0, bytes, 0, 2*xs.Length)
        bytes :?> byte[]

    let create freq =
        let samples = 12000
        let sine i = sin (Math.PI * 2. * float i / float sampleRate * freq)
        let fadeOut i = float (samples-i) / float samples
        Array.init samples (fun i -> sine i * fadeOut i |> sample)
        |> toBytes

    let play freq =        
        let bytes = create freq
        let effect = new SoundEffect(bytes, sampleRate, AudioChannels.Mono)       
        effect.Play() |> ignore
    
    let tones = [
        "A", 220.00
        "A#", 233.08
        "B", 246.94
        "C", 261.63
        "C#", 277.18
        "D", 293.66
        "D#", 311.13
        "E", 329.63
        "F", 349.23
        "F#", 369.99
        "G", 392.00
        "G#", 415.30
        "A", 440.00
        "A#", 466.16
        "B", 493.88
        "C", 523.25
        "C#", 554.37
        "D", 587.33]

    let keys =
        tones |> Seq.map (fun (text,freq) ->
            let key = Button(Content=text)
            key.Click.Add(fun _ -> play freq)
            key
        )

    let grid = Grid()
    do  keys |> Seq.iteri (fun i key ->
            grid.ColumnDefinitions.Add(ColumnDefinition())
            Grid.SetColumn(key,i)
            grid.Children.Add key
        )

    do  this.Startup.AddHandler(fun o e -> this.RootVisual <- grid)