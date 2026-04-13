# Bachelorarbeit - Belohnungsstrukturen im Reinforcement Learning

## Überblick

Die Belohnungsstruktur ist einer der wichtigsten Bausteine für effizientes Lernen von RL-Agenten, denn nur mit gutem Feedback weiß ein Agent, wann eine Aktion gut oder schlecht war.

Schon seit 30 Jahren wird an verschiedensten Belohnungsfunktionen geforscht. Manche Forscher geben den RL-Agenten nur dann eine Belohnung, wenn das Endziel erreicht wurde. Andere belohnen auch kleine Zwischenschritte, damit der Agent schneller versteht, in welche Richtung er sich bewegen soll. Wieder andere orientieren sich daran, wie ein Mensch die Aufgabe bewerten würde, und versuchen dieses Verhalten nachzubilden.
Dabei variiert die Performance der Agenten in den Experimenten je nach Aufgabe und Belohnungsfunktion.

Welche dieser Strategien funktioniert aber besser?

Genau das ist der Ausgangspunkt dieser Arbeit. Hier werden zwei unterschiedliche Belohnungsarten direkt miteinander verglichen, indem die Leistung von Agenten mit häufigem Feedback während des Lernens (dense rewards) der Leistung von Agenten mit sparsamen Feedback (sparse rewards) gegenübergestellt wird. Zusätzlich hierzu gibt es 3 verschiedene Schwierigkeitsgrade, um erforschen zu können, welche Art von Belohnungsfunktion bei welcher Schwierigkeitsstufe bessere Ergebnisse erzielt.

---

## Methodik

Um die zwei verschiedenen Belohnungsfunktionen zu vergleichen, wird natürlich auch eine Aufgabe benötigt, die von den Agenten erledigt werden soll.

Wie erwähnt gibt es 3 verschiedene Schwierigkeitsstufen, mit jeder Stufe erhöht sich:

- die Anzahl möglicher Aktionen der Agenten
- die Menge an Beobachtungen, die die Agenten erhalten
- Anzahl der Zustände, die die Agenten entdecken können

Da es so keine passende Aufgabe gab, mit der ich die Untersuchung starten könnte, habe ich mir ein eigenes Spiel überlegt und in Unity programmiert.

Das Prinzip des Spiels ist recht einfach:
Der Agent in der Mitte des Bildschirms verschießt Blitze und muss mit den Blitzen insgesamt 50 Fässer zerstören. Das Ziel des Spiels ist es, die Fässer so schnell wie möglich zu zerstören.
Dabei verursachen Blitze bei Fässern mit übereinstimmender Farbe mehr Schaden.
Mit steigendem Schwierigkeitsgrad kann der Agent seine Blitze durch Fähigkeitspunkte und mit kaufbaren Items verstärken.

Hier ist einmal eine Beispielszene aus dem Training im härtesten Schwierigkeitsgrad dargestellt.

<p align="center">
  <img src="images/training.gif" width="600"/>
</p>

kleine Beschreibung

---

## Ergebnisse

Die Ergebnisse zeigen deutlich, dass Agenten mit dichter Belohnungsfunktion signifikant bessere Lernergebnisse erzielen als Agenten mit spärlicher Belohnung.

Insbesondere mit steigender Aufgabenkomplexität verstärkt sich dieser Effekt:

- Agenten mit Sparse Rewards zeigen eine deutlich geringere Sample Efficiency
- Lernfortschritte sind langsamer und instabiler
- In komplexeren Umgebungen gelingt das Lernen teilweise nur eingeschränkt

Im Gegensatz dazu profitieren Agenten mit Dense Rewards von kontinuierlichem Feedback, was zu stabileren und effizienteren Lernprozessen führt.

---

## Visualisierung der Ergebnisse

<p align="center">
  <img src="images/graph1.png" width="500"/>
  <img src="images/graph2.png" width="500"/>
  <img src="images/graph3.png" width="500"/>
</p>

---

## Fazit

Mit steigender Schwierigkeit der Aufgabe fällt es Agenten zunehmend schwerer, effizient zu lernen, wenn sie nur selten Feedback erhalten.

Gleichzeitig wird klar, wie entscheidend eine gut gestaltete Belohnungsfunktion ist. Häufiges, gezieltes Feedback hilft dem Agenten, schneller sinnvolle Strategien zu entwickeln.
