USE Restaurant;
GO

INSERT INTO QuizEigenschappen
(ProductId, Zoet, Zout, Bitter, Fris, Pikant, Alcoholisch, Warm, Koud, Licht, Zwaar, Romig, Fruitig, Kruidig, Exotisch)
VALUES
-- Warme Dranken
(1, 0, 0, 1, 0, 0, 0, 2, 0, 1, 0, 0, 0, 0, 0), -- Espresso
(2, 1, 0, 1, 0, 0, 0, 2, 0, 1, 0, 1, 0, 0, 0), -- Cappuccino
(3, 0, 0, 1, 0, 0, 0, 2, 0, 1, 0, 0, 0, 1, 0), -- Thee Earl Grey

-- Koude Dranken
(4, 2, 0, 0, 2, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0), -- Cola
(5, 0, 0, 0, 2, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0), -- Spa Rood
(6, 2, 0, 0, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0, 0), -- Verse Jus d'Orange

-- Alcoholische Dranken
(7, 0, 1, 1, 1, 0, 2, 0, 2, 1, 1, 0, 0, 1, 0), -- Jupiler
(8, 0, 0, 1, 2, 0, 2, 0, 2, 1, 1, 0, 0, 1, 1), -- Gin Tonic

-- Wijnen
(9, 1, 0, 1, 1, 0, 2, 0, 2, 1, 1, 0, 1, 1, 1), -- Chardonnay
(10, 0, 0, 2, 0, 0, 2, 0, 2, 1, 2, 0, 0, 1, 1), -- Merlot

-- Voorgerechten
(11, 0, 1, 0, 1, 0, 0, 2, 0, 1, 1, 1, 0, 1, 0), -- Carpaccio
(12, 1, 1, 0, 1, 0, 0, 2, 0, 1, 1, 1, 0, 1, 0), -- Garnaalkroketjes

-- Hoofdgerechten
(14, 0, 2, 0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 1, 0), -- Steak met frietjes
(15, 0, 1, 0, 1, 0, 0, 2, 0, 1, 1, 0, 0, 1, 1), -- Zalmfilet
(16, 1, 1, 0, 1, 0, 0, 2, 0, 1, 1, 1, 0, 1, 0), -- Vegetarische Lasagne

-- Salades
(17, 0, 1, 0, 2, 0, 0, 0, 2, 2, 0, 0, 0, 1, 0), -- Caesar Salade
(18, 1, 0, 0, 2, 0, 0, 0, 2, 2, 0, 1, 1, 1, 0), -- Geitenkaas Salade

-- Pizza's
(19, 1, 1, 0, 0, 0, 0, 2, 0, 1, 1, 1, 0, 1, 0), -- Margherita
(20, 1, 1, 0, 0, 0, 0, 2, 0, 1, 1, 1, 0, 1, 0), -- Quattro Stagioni

-- Desserts
(21, 2, 0, 0, 0, 0, 1, 2, 0, 1, 1, 2, 1, 0, 1), -- Tiramisu
(22, 2, 0, 0, 0, 0, 0, 2, 0, 1, 1, 2, 1, 0, 0), -- Chocolademousse
(23, 2, 0, 0, 0, 0, 0, 2, 0, 1, 1, 2, 1, 0, 0), -- Dame Blanche

-- Chef's Special
(24, 0, 1, 0, 1, 0, 2, 2, 0, 1, 2, 0, 0, 2, 2), -- Lobster Thermidor
(25, 0, 2, 0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 1, 2); -- Wagyu Beef