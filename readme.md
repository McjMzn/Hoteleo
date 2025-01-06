# Hoteleo App
## Usage
### Build application

`dotnet build` (requires .NET 8.0 SDK)

### Launch application providing the required arguments
`./Hoteleo/bin/Debug/net8.0/Hoteleo.exe --hotels <hotels_json_file_path> --bookings <bookings_json_file_path>`

### Run commands
- `Search(<hotel_id>, <days_ahead>, <room_type>)`
  - Arguments:
    - hotel_id: string
    - room_type: string
    - days_ahead: integer
- `Availability(<hotel_id>, <date>, <room_type>)`
  - Arguments:
    - hotel_id: string
    - date:
      - either: `yyyyMMdd` formatted string representing single point in time
      - or: `yyyyMMdd-yyyyMMdd` representing dates range
    - room_type: string
- `<empty_line>` to exit


### Input files examples
**hotels.json**
```json
[
  {
    "id": "ExampleHotel",
    "rooms": [
      { "roomType": "ExampleRoom", "roomId": "101" },
      { "roomType": "ExampleRoom", "roomId": "102" },
    ]
  }
]
```
**bookings.json**
```json
[
  {
    "hotelId": "ExampleHotel",
    "arrival": "20241201",
    "departure": "20241205",
    "roomType": "ExampleRoom",
  }
]
```