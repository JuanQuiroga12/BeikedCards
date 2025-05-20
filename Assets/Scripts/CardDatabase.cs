using System.Collections.Generic;
using UnityEngine;

public static class CardDatabase
{
    // Add this to CardDatabase.cs
    private static Dictionary<string, string> storyNames = new Dictionary<string, string>
{
    { "story_1", "Cr�nicas de una familia dulce" },
    { "story_2", "El c�rculo de las galletas encantadas" },
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
        // Story 1: Cr�nicas de una familia dulce
        { "1-Common-Cinnamon", "Cinnamon Roll" },
        { "1-Common-Klim", "Klim brigadeiro" },
        { "1-Common-Kinder", "Kinder" },
        { "1-Strange-Birthday", "Birthday Cake" },
        { "1-Strange-Milo", "Milo" },
        { "1-Deluxe-BrowniePistacho", "Brownie Pistachio" },
        
        // Story 2: El c�rculo de las galletas encantadas
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
        // Story 1: Cr�nicas de una familia dulce
        { "1-Common-Cinnamon", "Nos encontramos por casualidad, compartiendo un cinnamon roll y una conversaci�n que nunca quisimos terminar. Desde ese momento, supimos que lo nuestro iba a ser especial. �ramos dos almas listas para escribir una historia juntos." },
        { "1-Common-Klim", "Cuando supimos que vendr�as al mundo, la vida se volvi� m�s suave, como leche tibia en las ma�anas. Esperarte fue como so�ar despiertos: prepararnos, re�r, llorar, imaginar c�mo ser�a tenerte en nuestros brazos." },
        { "1-Common-Kinder", "Tu llegada fue como abrir el regalo m�s tierno del universo. Peque�o, con los ojos llenos de futuro, nos miraste y entendimos que hab�as cambiado nuestra historia para siempre. �ramos tres, y todo ten�a m�s sentido." },
        { "1-Strange-Birthday", "Tus primeros pasos, tus primeras palabras, tus primeras fiestas llenas de pastel y risas. Cada a�o contigo era una nueva aventura, y cada abrazo, un recordatorio de que el amor crece contigo." },
        { "1-Strange-Milo", "Creciste, y con cada taza de Milo compartida aprendimos que ser familia es elegirnos todos los d�as. Mirarte es ver nuestro amor convertido en persona. Y esta historia, la tuya, la nuestra, apenas comienza." },
        { "1-Deluxe-BrowniePistacho", "Full art" },
        
        // Story 2: El c�rculo de las galletas encantadas
        { "2-Common-ChocolateChips", "Las migas de alegr�a se fundieron con granos de cacao antiguo, naciendo as� la Classic Chocolate Chips, quien aprendi� a guiar con dulzura y templanza. Esta galleta entendi� que *sin contraste, la alegr�a pierde su brillo, y por eso busc� a la �nica capaz de unir lo opuesto..." },
        { "2-Common-KeyLimePie", "Key Lime Pie naci� entre rayos c�tricos y confusi�n. Apareci� cuando la dulzura y la contradicci�n colisionaron. Su poder de alterar la realidad desat� recuerdos que nunca existieron y futuros a�n no escritos. Ella revel� un secreto: el origen de todas las galletas ten�a un alma com�n." },
        { "2-Common-CaramelPecans", "Nacida de una tormenta de sal y caramelo, esta galleta dom� la contradicci�n. Al conocer la historia de Birthday Cake y Classic Chips, comprendi� que deb�a *reconciliar lo dulce, lo salado, lo tierno y lo fuerte. Pero al hacerlo, cre� un v�rtice de energ�a tan inestable que desgarr� el cielo..." },
        { "2-Strange-Birthday", "Nacida en la cima de la Monta�a de las Velas Infinitas, la galleta Birthday Cake contiene el Primer Deseo pronunciado en el reino. Ese deseo dio origen a Beikedia. Pero la alegr�a pura no pod�a sobrevivir sola, por lo que la galleta cre� un mapa emocional con migas encantadas que guiaron a otra guardiana." },
        { "2-Strange-Klim", "Nacida en la cima de la Monta�a de las Velas Infinitas, la galleta Birthday Cake contiene el Primer Deseo pronunciado en el reino. Ese deseo dio origen a Beikedia. Pero la alegr�a pura no pod�a sobrevivir sola, por lo que la galleta cre� un mapa emocional con migas encantadas que guiaron a otra guardiana." },
        { "2-Deluxe-Nutella", "Full art" },
        
        // Story 3: El despertar de Campfire
        { "3-Common-ChocolateChips", "Fui el primero en sentirlo: una se�al del viejo horno. El Relleno Dorado, esencia de nuestras recetas, hab�a desaparecido. Pero no era solo eso... so�� con una galleta jam�s horneada: Red Velvet Hershey's Campfire. Dicen que solo aparece a quienes re�nen el Relleno completo. As� comenz� mi viaje." },
        { "3-Common-Cinnamon", "Cada espiral de mi cuerpo guarda visiones dulces del pasado. Cuando sent� la fragancia de Chip y Keyla, supe que el momento hab�a llegado. El Relleno deb�a volver... pero no solo por nosotros. Una galleta dorm�a en las llamas del olvido. Red Velvet. Y el mundo deb�a saborearla." },
        { "3-Common-KeyLimePie", "Mi corteza guarda secretos, y entre mis hojas de lima escondo mapas olvidados. Cuando Chip lleg�, su b�squeda me intrig�: no por el Relleno, sino por esa leyenda crujiente... una galleta con alma de fuego y chocolate. Campfire. Juntos, partimos en su b�squeda." },
        { "3-Strange-LaDeMilo", "No dejo que cualquiera me muerda el orgullo. Pero ese tr�o ten�a algo... fe. Me vencieron con sabor y raz�n. Dentro de m� ard�a un fragmento del Relleno Dorado, que jur� proteger. Pero si eso despierta a la Red Velvet Hershey's Campfire... entonces vale cada miga." },
        { "3-Strange-MoltenLava", "Fui creada del cacao m�s oscuro y el fuego m�s intenso. Desde que el Relleno desapareci�, nada volvi� a derretirme... hasta que Chip habl� de ella: una galleta olvidada entre brasas y recuerdos, Red Velvet Hershey's Campfire. Le di el fragmento. Porque quiero verla... una vez m�s." },
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
        return "Una carta de la colecci�n BeikCookie"; // Default fallback description
    }
}