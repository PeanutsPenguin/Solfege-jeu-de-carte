using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Enum afin de definir les note 
/// </summary>
public enum E_NOTE : int
{
    E_DO = 0, 
    E_RE,
    E_MI, 
    E_FA, 
    E_SOL,
    E_LA,
    E_SI
}

namespace NoteValues
{
    class NoteValuesHandler
    {
        /// <summary>
        /// Retourne le bon texte en fonction de la note
        /// </summary>
        public static string SetNoteText(E_NOTE note)
        {
            switch (note)
            {
                case E_NOTE.E_DO:
                    return "DO";
                case E_NOTE.E_RE:
                    return "RE";
                case E_NOTE.E_MI:
                    return "MI";
                case E_NOTE.E_FA:
                    return "FA";
                case E_NOTE.E_SOL:
                    return "SOL";
                case E_NOTE.E_LA:
                    return "LA";
                case E_NOTE.E_SI:
                    return "SI";
            }

            return "DO";
        }

        /// <summary>
        /// Retourne la bonne couleur en fonction de la note (base sur les couleurs de l'arc en ciel)
        /// </summary>
        public static Color setNoteColor(E_NOTE note)
        {
            switch (note)
            {
                case E_NOTE.E_DO:
                    return Color.red;
                case E_NOTE.E_RE:
                    return Color.orange;
                case E_NOTE.E_MI:
                    return Color.yellow;
                case E_NOTE.E_FA:
                    return Color.green;
                case E_NOTE.E_SOL:
                    return Color.blue;
                case E_NOTE.E_LA:
                    return Color.indigo;
                case E_NOTE.E_SI:
                    return Color.purple;
            }

            return Color.red;
        }
    }
}
