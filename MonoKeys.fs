namespace MonoKeys

open System
open System.Windows
open System.Windows.Controls
open Microsoft.Xna.Framework.Audio

type App() as this = 
    inherit Application()

    let sampleRate = 44100
    let sampleLength = 1.5 * float sampleRate |> int
    let sample x = x * 32767. |> int16

    let toBytes (xs:int16[]) =
        let bytes = Array.CreateInstance(typeof<byte>, 2 * xs.Length)
        Buffer.BlockCopy(xs, 0, bytes, 0, 2*xs.Length)
        bytes :?> byte[]

    let pi = Math.PI
    let sineWave freq i = sin (pi * 2. * float i / float sampleRate * freq)
    let fadeOut i = float (sampleLength-i) / float sampleLength
    let tremolo freq depth i = (1.0 - depth) + depth * Math.Pow(sineWave freq i, 2.0)

    let create f =
        Array.init sampleLength (f >> min 1.0 >> max -1.0 >> sample)
        |> toBytes

    let play bytes =
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

    let grid = Grid()
    do  grid.RowDefinitions.Add(RowDefinition(Height=GridLength.Auto))
    do  grid.RowDefinitions.Add(RowDefinition())
    do  grid.RowDefinitions.Add(RowDefinition())

    let tremoloFreq = Slider(Minimum = 0.001, Maximum = 10.0, Value=2.0)
    do  Grid.SetRow(tremoloFreq, 1)
    let tremoloDepth = Slider(Minimum = 0.0, Maximum = 50.0, Value=10.0)
    do  Grid.SetRow(tremoloDepth, 2)

    let keys =
        tones |> Seq.map (fun (text,freq) ->
            let key = Button(Content=text)            
            key.Click.Add(fun _ ->
                let tremolo i = tremolo tremoloFreq.Value tremoloDepth.Value i
                let f i = sineWave freq i * fadeOut i * tremolo i
                let bytes = create f
                play bytes
            )
            key
        )

    let keyGrid = Grid(Height=128.)
    do  keys |> Seq.iteri (fun i key ->
            keyGrid.ColumnDefinitions.Add(ColumnDefinition())
            Grid.SetColumn(key,i)
            keyGrid.Children.Add key
        )

    do  grid.Children.Add keyGrid
    do  grid.Children.Add tremoloFreq
    do  grid.Children.Add tremoloDepth

    do  this.Startup.AddHandler(fun o e -> this.RootVisual <- grid)