# SolfegeGame
<details>
  <summary>Sommaire</summary>
  <ol>
    <li>
      <a href="#apprends-le-solfège-en-jouant-">Apprends le solfège en jouant !</a>
    </li>
    <li>
      <a href="#première-itération">Première itération</a>
    </li>
    <li>
      <a href="#seconde-itération">Seconde itération</a>
    </li>
    <li>
      <a href="#troisième-itération">Troisième itération</a>
    </li>
    <li>
      <a href="#quatrième-et-dernière-itération">Quatrième et dernière itération</a>
    </li>
    <li>
      <a href="#midihandler">Midi Handler</a>
    </li>
    <li>
      <a href="#conclusion">Conclusion</a>
    </li>
    <li>
      <a href="#contact">Contact</a>
    </li>
  </ol>
</details>


## Apprends le solfège en jouant !
Ce projet a pour but de créer un mini jeu qui permet d'apprendre le solfège dans un environement ludique. 
Il est fait avec unity 2D et DryWetMIDI afin de lire des fichier MIDI et de les jouer !

## Première itération
La première idée qui m'est venu en tête était de crée une sorte de jeu de carte ou il était possible de les reliés au bon emplacement dans la portée. J'ai donc developpé un premier prototype. 

https://github.com/user-attachments/assets/3fec137b-a636-4789-bf27-747eda77b167

## Seconde itération
J'ai vite compris que cela n'était pas très clair donc j'ai decide de rajouter un code couleur et des explications afin de comprendre que ce qu'il fallait faire.\
<img width="1919" height="1079" alt="SecondIteration" src="https://github.com/user-attachments/assets/49b8199f-28d9-4ee1-9c73-16b1712ee64f" />

## Troisième itération
Suite a tout ca il a fallu rendre la chose ludique tout en apprenant. Je suis donc parti sur une base de 4 niveau :

- Niveau 1 : Relier les notes avec code couleur + texte affiche
- Niveau 2 : Relier les notes avec uniquement le code couleur
- Niveau 3 : Relier les notes sans aucune aide 
- Niveau 4 : Jouer librement chaque note

## Quatrième et dernière itération
Après toute ces itératons, je trouvais que le jeu n'était tout de même pas très clair (et pas très beau) j'ai donc décidé de refactor le UI afin de rendre tout ça plus lisible et plus joli.
Voici donc le rendu final du jeu : 

https://github.com/user-attachments/assets/de4bbdb5-e142-451a-a8a6-d2a533c55141

Ce n'est que le niveau 1 donc n'hesitez pas à installer la build et tester par vous même !

## MidiHandler
Ce que toutes ces itérations ne montrent pas c'est le MidiHandler. La base ce ce projet est de pouvoir lire des fichiers Midi et de les jouer. Un fichier Midi est un fichier contenant la "data" d'une musique : 
quelle note est jouée, à quel moment, à quelle intensité, quel octave, le bpm de la musique ect...\
Ce programme permet donc de lire et jouer ce genre de fichier (en utilisant la librairie DryWetMidi).
Lorsqu'un fichier est joué je récupère chaque évènement et si une note doit être joué alors je lance un son en fonction de la note !
N'hésitez pas à regarder le script MidiHandler.cs pour voir comment je procède.

## Conclusion
Ce projet m'a permis d'en apprendre plus sur Unity et la librairie DryWetMidi. De plus il reste un projet très "scalable", beaucoup de feature peuvent être ajouté par dessus (notes dièses, octaves différents, autres clés) donc peut être que je continuerais ! Merci d'avoir lu !

## Contact

<u>**Malo Sadoine**</u>

- [![Linkedin][LinkedIn]][LinkedIn-url]
- mal.sadoine@gmail.com

[LinkedIn]: https://img.shields.io/badge/linkedin-34a8eb?style=for-the-badge&logo=linkedin
[LinkedIn-url]: https://www.linkedin.com/in/malo-sadoine-098b7a254/
