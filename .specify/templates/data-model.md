
### Key Entities (Data Models)

| Entity Name | Description | Type | Attributes |
| :--- | :--- | :--- | :--- |
| **SpecialEvent** | The core entity representing a museum SpecialEvent. | `object` | \<ul\>\<li\>`EventId`: **UUID**, Unique identifier.\</li\>\<li\>`EventName`: **string**, Human-readable name.\</li\>\<li\>`EventDescription`: **string**, Detailed content description.\</li\>\<li\>`EventDates`: **array**, Dates event appears.\</li\>\<li\>`Price`: **decimal**, Must be positive (FR-003 enforced).\</li\>\</ul\> |
| **SpecialEventCollection** | List of upcoming events. | `array` | *Items:* **SpecialEvent** |
| **MuseumDailyHours** | The hours of operations for a day. | `object` | \<ul\>\<li\>`Date`: **date**\</li\>\<li\>`TimeOpen`: **time**\</li\>\<li\>`TimeClosed`: **time**\</li\>\</ul\> |
| **MuseumHours** | The hours of operations for a date range. | *Items:* **MuseumDailyHours** | \<ul\>\<li\>`Date`: **date**, Date operating hours apply to.\</li\>\<li\>`TimeOpen`: **time**\</li\>\<li\>`TimeClosed`: **time**\</li\>\</ul\> |
| **Ticket** | Ticket for museum entry (can be general or special event). | `object` | \<ul\>\<li\>`TicketId`: **UUID**, Unique identifier.\</li\>\<li\>`TicketDate`: **date**, General or Special Event Date.\</li\>\<li\>`TicketType`: **string**, General or Special Event.\</li\>\<li\>`EventId`: **EventId**, Only required if purchasing a Special Event ticket.\</li\>\</ul\> |
| **MuseumTicketConfirmation** | Details for a museum ticket after a successful purchase. | `object` | \<ul\>\<li\>`TicketCodeImage`: **string**, Image of ticket with a QR code used for museum or event entry.\</li\>\<li\>`MuseumHours`: **MuseumHours**, Operating Hours of date of ticket.\</li\>\<li\>`TicketType`: **string**, General or Special Event.\</li\>\<li\>`EventId`: **UUID**, Only required if purchasing a Special Event ticket.\</li\>\</ul\> |
| **BuyMuseumTickets** | Data to purchase a ticket. | `object` | \<ul\>\<li\>`TicketMessage`: **string**, Confirmation message.\</li\>\<li\>`TicketId`: **UUID**, Unique identifier.\</li\>\<li\>`TicketConfirmation`: **string**, Unique code used to verify ticket purchase.\</li\>\</ul\> |
| **Error** | Standard error object. | `object` | \<ul\>\<li\>`Type`: **string**\</li\>\<li\>`Title`: **string**, Validation Failed.\</li\>\</ul\> |

