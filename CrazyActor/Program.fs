open Akkling

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

type ResultEffect<'Message> (value) =
    member val Value = value with get
    interface Effect<'Message> with
        member this.WasHandled () =
            false
        member this.OnApplied(context : ExtActor<'Message>, message : 'Message) = 
            ()


let myActor = (props(fun mailbox ->
    printfn "Actor started"
    let rec loop state = 
        actor {
            
            printfn "Start of loop"

            let! anyMessage = mailbox.Receive()
            printfn "Got a message %O" anyMessage

            let! (something:ResultEffect<_>) = actor {
                // do some work
                return! ResultEffect("Hello World!")
            }

            let! abc = ResultEffect("Hello World!")

            let! abc = Ignore

            let value = something.Value
            printfn "Value was: %s" value
            
            printfn ""
            return! loop ()
        }
    loop () :> _ ))

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let configuration = Configuration.defaultConfig ()
    let system = System.create "system" configuration

    let aref = spawn system "Actor1" myActor
    aref <! ("NopeRope" :> obj)


    system.WhenTerminated |> Async.AwaitTask |> Async.RunSynchronously
    0 // return an integer exit code
