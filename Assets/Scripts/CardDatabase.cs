using System.Collections.Generic;
using UnityEngine;

public static class CardDatabase
{
    // Add this to CardDatabase.cs
    private static Dictionary<string, string> storyNames = new Dictionary<string, string>
{
    { "story_1", "Crónicas de una familia dulce" },
    { "story_2", "El círculo de las galletas encantadas" },
    { "story_3", "El Despertar de Campfire" }
};

    public static string GetStoryName(string storyId)
    {
        if (storyNames.TryGetValue(storyId, out string name))
        {
            return name;
        }
        return "Historia Desconocida";
    }

    // Dictionary mapping prefab names to card names
    private static Dictionary<string, string> cardNames = new Dictionary<string, string>
    {
        // Story 1: Crónicas de una familia dulce
        { "1-Common-Cinnamon", "Cinnamon Roll" },
        { "1-Common-Klim", "Klim brigadeiro" },
        { "1-Common-Kinder", "Kinder" },
        { "1-Strange-Birthday", "Birthday Cake" },
        { "1-Strange-Milo", "Milo" },
        { "1-Deluxe-BrowniePistacho", "Brownie Pistachio" },
        
        // Story 2: El círculo de las galletas encantadas
        { "2-Common-ChocolateChips", "Chocolate Chips" },
        { "2-Common-KeyLimePie", "Key lime pie" },
        { "2-Common-CaramelPecans", "Caramel Pecans" },
        { "2-Strange-Birthday", "Birthday Cake" },
        { "2-Strange-Klim", "Klim Brigadeiro" },
        { "2-Deluxe-Nutella", "Nutella" },
        
        // Story 3: El despertar de Campfire
        { "3-Common-ChocolateChips", "Chocolate Chips" },
        { "3-Common-Cinnamon", "Cinnamon Roll" },
        { "3-Common-KeyLimePie", "Key Lime Pie" },
        { "3-Strange-LaDeMilo", "La de Milo" },
        { "3-Strange-MoltenLava", "Molten Lava" },
        { "3-Deluxe-HCampfire", "Campfire" }
    };

    // Dictionary mapping prefab names to card descriptions
    private static Dictionary<string, string> cardDescriptions = new Dictionary<string, string>
    {
        // Story 1: Crónicas de una familia dulce
        { "1-Common-Cinnamon", "Nos encontramos por casualidad, compartiendo un cinnamon roll y una conversación que nunca quisimos terminar. Desde ese momento, supimos que lo nuestro iba a ser especial. Éramos dos almas listas para escribir una historia juntos." },
        { "1-Common-Klim", "Cuando supimos que vendrías al mundo, la vida se volvió más suave, como leche tibia en las mañanas. Esperarte fue como soñar despiertos: prepararnos, reír, llorar, imaginar cómo sería tenerte en nuestros brazos." },
        { "1-Common-Kinder", "Tu llegada fue como abrir el regalo más tierno del universo. Pequeño, con los ojos llenos de futuro, nos miraste y entendimos que habías cambiado nuestra historia para siempre. Éramos tres, y todo tenía más sentido." },
        { "1-Strange-Birthday", "Tus primeros pasos, tus primeras palabras, tus primeras fiestas llenas de pastel y risas. Cada año contigo era una nueva aventura, y cada abrazo, un recordatorio de que el amor crece contigo." },
        { "1-Strange-Milo", "Creciste, y con cada taza de Milo compartida aprendimos que ser familia es elegirnos todos los días. Mirarte es ver nuestro amor convertido en persona. Y esta historia, la tuya, la nuestra, apenas comienza." },
        { "1-Deluxe-BrowniePistacho", "Full art" },
        
        // Story 2: El círculo de las galletas encantadas
        { "2-Common-ChocolateChips", "Las migas de alegría se fundieron con granos de cacao antiguo, naciendo así la Classic Chocolate Chips, quien aprendió a guiar con dulzura y templanza. Esta galleta entendió que *sin contraste, la alegría pierde su brillo, y por eso buscó a la única capaz de unir lo opuesto..." },
        { "2-Common-KeyLimePie", "Key Lime Pie nació entre rayos cítricos y confusión. Apareció cuando la dulzura y la contradicción colisionaron. Su poder de alterar la realidad desató recuerdos que nunca existieron y futuros aún no escritos. Ella reveló un secreto: el origen de todas las galletas tenía un alma común." },
        { "2-Common-CaramelPecans", "Nacida de una tormenta de sal y caramelo, esta galleta domó la contradicción. Al conocer la historia de Birthday Cake y Classic Chips, comprendió que debía *reconciliar lo dulce, lo salado, lo tierno y lo fuerte. Pero al hacerlo, creó un vórtice de energía tan inestable que desgarró el cielo..." },
        { "2-Strange-Birthday", "Nacida en la cima de la Montaña de las Velas Infinitas, la galleta Birthday Cake contiene el Primer Deseo pronunciado en el reino. Ese deseo dio origen a Beikedia. Pero la alegría pura no podía sobrevivir sola, por lo que la galleta creó un mapa emocional con migas encantadas que guiaron a otra guardiana." },
        { "2-Strange-Klim", "Nacida en la cima de la Montaña de las Velas Infinitas, la galleta Birthday Cake contiene el Primer Deseo pronunciado en el reino. Ese deseo dio origen a Beikedia. Pero la alegría pura no podía sobrevivir sola, por lo que la galleta creó un mapa emocional con migas encantadas que guiaron a otra guardiana." },
        { "2-Deluxe-Nutella", "Full art" },
        
        // Story 3: El despertar de Campfire
        { "3-Common-ChocolateChips", "Fui el primero en sentirlo: una señal del viejo horno. El Relleno Dorado, esencia de nuestras recetas, había desaparecido. Pero no era solo eso... soñé con una galleta jamás horneada: Red Velvet Hershey's Campfire. Dicen que solo aparece a quienes reúnen el Relleno completo. Así comenzó mi viaje." },
        { "3-Common-Cinnamon", "Cada espiral de mi cuerpo guarda visiones dulces del pasado. Cuando sentí la fragancia de Chip y Keyla, supe que el momento había llegado. El Relleno debía volver... pero no solo por nosotros. Una galleta dormía en las llamas del olvido. Red Velvet. Y el mundo debía saborearla." },
        { "3-Common-KeyLimePie", "Mi corteza guarda secretos, y entre mis hojas de lima escondo mapas olvidados. Cuando Chip llegó, su búsqueda me intrigó: no por el Relleno, sino por esa leyenda crujiente... una galleta con alma de fuego y chocolate. Campfire. Juntos, partimos en su búsqueda." },
        { "3-Strange-LaDeMilo", "No dejo que cualquiera me muerda el orgullo. Pero ese trío tenía algo... fe. Me vencieron con sabor y razón. Dentro de mí ardía un fragmento del Relleno Dorado, que juré proteger. Pero si eso despierta a la Red Velvet Hershey's Campfire... entonces vale cada miga." },
        { "3-Strange-MoltenLava", "Fui creada del cacao más oscuro y el fuego más intenso. Desde que el Relleno desapareció, nada volvió a derretirme... hasta que Chip habló de ella: una galleta olvidada entre brasas y recuerdos, Red Velvet Hershey's Campfire. Le di el fragmento. Porque quiero verla... una vez más." },
        { "3-Deluxe-HCampfire", "Full art" }
    };

    // Get the real name of a card by its prefab name
    public static string GetCardName(string prefabName)
    {
        if (cardNames.TryGetValue(prefabName, out string name))
        {
            return name;
        }
        return prefabName; // Fallback to prefab name if not found
    }

    // Get the description of a card by its prefab name
    public static string GetCardDescription(string prefabName)
    {
        if (cardDescriptions.TryGetValue(prefabName, out string description))
        {
            return description;
        }
        return "Una carta de la colección BeikCookie"; // Default fallback description
    }
}