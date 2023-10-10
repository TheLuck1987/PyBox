# PyBox
Una "semplice" prova tecnica

Requisiti:
* Progettazione, realizzazione e test di una app basata su C# .NET di ultima generazione con le seguenti features:
  - REST API Endpoints necessari allo scopo (SOLO API, NO UI)
  - Possibilità di inviare, editare e cancellare codice sorgente in linguaggio Pyton e registrarlo come script entry in una collection persistente (Files, DB, etc..)
  - Possibilità di monitorare e cambiare lo stato di esecuzione dello script (Enabled/disabled)
  - Possibilità di richiamare uno script con una lista di argomenti, passarla allo script ed eseguirlo, restituire il risultato dell’esecuzione.
  - Progettazione, realizzazione e test di una app basata su C# .NET di ultima generazione con le seguenti features:
* UI WEB basata su Blazor Server Side
  - Uso di dependency injection
  - Pagina e componenti razor relativi a Visualizzazione degli script caricati, con relativo stato e interazione
  - Pagina e componenti razor necessari per realizzare una sandbox dello script per poterlo richiamare, visualizzare, modificare (Monaco), ed eseguire con input di argomenti e output a schermo.

Note generali:
  - Questa versione è puramente dimostrativa ed è servita anche a me per studiare e approfondire (anche se abbastanza marginalmente) concetti, tecniche e frameworks a me poco conosciuti.
  - Anche se l'applicazione richiedeva un semplice database contenente una singola tabella, ho utilizzato EntityFramework (che non avevo mai utilizzato)
  - Questa è stata la prima volta in cui utilizzavo Blazor e Razor quindi, molto probabilmente, non ho seguito tutte le Best Practice al 100% e il tutto potrebbe essere sicuramente migliorato, semplificato e ottimizzato.
