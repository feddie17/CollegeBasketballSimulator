# College Basketball Simulator

A .NET/C# application that simulates NCAA college basketball at three levels — a single game, a full season, or an entire tournament bracket — using real statistical data pulled from the [Bart Torvik](https://barttorvik.com/) API. Runs locally with a browser-based UI for viewing simulations as they happen. (Not yet deployed — run locally to try it out.)

## How it works

Team and player statistics are pulled live from the Bart Torvik API and used to drive a **possession-by-possession simulation engine**. Rather than generating a final score directly, each possession is simulated individually, with outcomes (scoring, turnovers, rebounds, etc.) weighted by probabilities derived from the pulled statistics. Scores are built up organically over the course of a simulated game, the same way a real game unfolds possession to possession.

## Features

- **Single Game Simulation** — pick any two teams and simulate a head-to-head matchup
- **Season Simulation** — select a team and simulate its entire season game by game
- **Tournament Bracket Simulation** — build and simulate a full NCAA tournament bracket
- **Live UI Updates** — simulations run locally with scores updating in real time in a browser-based UI as the game/season/bracket plays out, rather than just returning a final result

## Technical Highlights

- Integration with an external, real-world statistical data API (Bart Torvik) rather than static/hardcoded data
- Possession-based probabilistic simulation engine, as opposed to a simple score-generation formula — models the actual flow of a basketball game
- Real-time, browser-based UI that reflects simulation state as it progresses, not just a final output
- Supports three distinct simulation modes (game / season / tournament) built on the same underlying engine

## Running Locally

This project is not currently deployed. To try it out, clone the repo and run it locally — the simulation UI is viewed in your browser once the app is running.

## Tech Stack

C# / .NET
