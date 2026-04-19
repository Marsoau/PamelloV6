# PamelloV6

> *Not intended for public usage, unlike a newer [PamelloV7](https://github.com/Marsoau/PamelloV7)*

**A 6th iteration of Pamello discord bot**

First API-first iteration, with actuall Player/Speaker split, and a database to store users, songs, episodes, and playlists

Also first to use "song values", which were way to interpret string with url/id/association/name/value to a song, allowed stuff like "random" to get random song from a database

Had **Angular Frontend** *(PamelloV7.Web project)* to controll the bot from a web interface

> *Date of initial creation: **June 2024***

---

**Previous iteration:** [PamelloV5](https://github.com/Marsoau/PamelloV5)

**Next iteration:** [PamelloV7](https://github.com/Marsoau/PamelloV7)

---

I will write about it here because the **PamelloV7** README is more about it features then differences from **PamelloV6**

## PamelloV7 1.0

Fixed **PamelloV6** instability, had better discord commands, and better API

> *Date of initial creation: **November 2024***

## PamelloV7 2.0

Could have very well been 8th iteration, but instead of full rewrite it was done in partial rewrites of **PamelloV7 1.0**, adding **more difference beween 2.0 and 1.0 of PamelloV7 then beween any iteration before**

1.0 achieved better stability compared to V6, but now i had a lot of ideas of how it can be even better, like:
- "A more powerfull song values"
- "Independence from discord and youtube"
- "Modular approach to allow extension, and allow for more decoupled approach in general"

2.0 then turned out to achieve:
- "A more powerfull song values" became PEQL: Pamello Entity Query Language, from an idea of "song value" being url/id/association/name/value of a song specifically, to a language desighned for ease and comfort of use, and as much flexibility sa possible without giving up simplicity
- Framework approach allowed to to split .Server and .Core projects to .Server, .Framework, .Core, and Modules. Also implemented a lot of Framework services to use in .Server and Modules
- Implemented modules that were compiled to .pv7m files and loaded by a .Server on runtime. All modules can fully interact with server via PamelloV7.Framework
- "Independence from discord and youtube" was solved by Modules and platform service, that takes on itself a responisibility to find a platform implementation and use it to get song info / download a song, the each platform support was then implemented as its own Module
- Audio routing system to provide for a developer audio modules like sources, pums, buffers, converters, to create stuff like speakers and other audio dependants with ease
- Started using Reflection & Source Generation a lot to achieve quite good developer expirience in **PamelloV7.Framework**
- **Marsoau/NetCord** discord module based on NetCord, extending it a lot with custom interactions, builders, and tokenization for button & modal interactions, resulting in arguably best-in-class DX in working with discord

And there is still many big areas to implement, like **PamelloV7.Client**, **PamelloV7.Launcher**, and more. So there is still some stuff to do in future updates for a long time

> *Date of initial creation: **May 2025***

> *Date of first release: **Soon***
