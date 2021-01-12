# Process

- Client scans the qr code
- Client connects to server via websocket. All further communication is through websocket.
- Server tells client it's ready
- User clicks ready on client when the device is in a static position in view of the camera
- Server commands client screen to blink at 10 hz
- Server finds pixels blinking at correct rate
- Server sets client to constant black
- Server saves current time
- Server commands client to change to white
- Client repeats the command back to server
- Server saves round trip time (rtt)
- When client color changes, server calculates delay


# Client

- Websocket
- Canvas
- requestFullScreen

# Server

- WPF App
- Microsoft dependency injection
- Embedded Asp Net Core server to serve client
- Display QR Code containing link to client
- Shows whats happening with the client
- Configuration through dotenv
- Optional SSL Cert