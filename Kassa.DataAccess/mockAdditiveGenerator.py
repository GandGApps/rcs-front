﻿import json
import random

# Перегенерация JSON-объектов для добавок с учетом того, что одна добавка может быть для нескольких продуктов

# Обновленный список добавок
updated_additions = {
    "Маршмеллоу": [18],
    "Шоколадные стружки": [18, 6, 27, 29],
    "Взбитые сливки": [18, 24, 25, 26],
    "Сироп карамельный": [19, 24, 25, 26],
    "Сироп ванильный": [19, 24, 25, 26],
    "Сироп шоколадный": [19, 24, 25, 26],
    "Орехи ассорти": [6, 27, 29],
    "Изюм": [6, 27, 29],
    "Кокосовая стружка": [6, 27, 29],
    "Сливки для кофе": [4],
    "Сахарный песок": [4],
    "Коричневый сахар": [4],
    "Соус барбекю": [42, 43, 44],
    "Горчица дижонская": [42, 43, 44],
    "Соус чесночный": [42, 43, 44],
    "Карамельные кусочки": [6, 27, 29],
    "Миндальные лепестки": [6, 27, 29],
    "Клубничный сироп": [18, 24, 25, 26],
    "Мятный сироп": [19, 24, 25, 26],
    "Лимонный сок": [10, 11],
    "Имбирь": [3, 20],
    "Мед": [3, 20, 7, 8],
    "Корица": [3, 20, 6],
    "Сушеная клюква": [6, 27, 29],
    "Арахисовая паста": [6, 27, 29],
    "Соевый соус": [42, 43, 44],
    "Терияки соус": [42, 43, 44],
    "Сырный соус": [42, 43, 44],
    "Перец чили": [42, 43, 44],
    "Луковые колечки": [42, 43, 44],
    "Кетчуп": [42, 43, 44],
    "Майонез": [42, 43, 44],
    "Гранатовый сироп": [18, 24, 25, 26],
    "Кедровые орешки": [6, 27, 29],
    "Сушеные абрикосы": [6, 27, 29],
    "Ванильный экстракт": [18, 6, 27, 29],
    "Ореховый крем": [6, 27, 29],
    "Лавандовый сироп": [19, 24, 25, 26],
    "Тростниковый сахар": [4],
    "Соус хойсин": [42, 43, 44],
    "Соус сатэ": [42, 43, 44],
    "Томатный соус": [42, 43, 44],
    "Соус песто": [42, 43, 44],
    "Зеленый лук": [42, 43, 44],
    "Чеснок молотый": [42, 43, 44],
    "Паприка": [42, 43, 44],
    "Сыр пармезан": [42, 43, 44],
    "Базилик": [42, 43, 44],
    "Тимьян": [42, 43, 44],
    "Розмарин": [42, 43, 44],
    "Майоран": [42, 43, 44],
    "Уксус бальзамический": [42, 43, 44],
    "Соус чили": [42, 43, 44],
    "Маринованные огурцы": [39, 40, 41],
    "Соленые каперсы": [39, 40, 41],
    "Смесь итальянских трав": [42, 43, 44],
    "Лимонная цедра": [18, 24, 25, 26],
    "Апельсиновая цедра": [18, 24, 25, 26],
    "Коньяк": [42, 43, 44],
    "Виски": [42, 43, 44],
    "Красное вино": [42, 43, 44],
    "Белое вино": [42, 43, 44],
    "Соевые проростки": [42, 43, 44],
    "Семена кунжута": [42, 43, 44],
    "Соус табаско": [42, 43, 44],
    "Оливковое масло": [42, 43, 44],
    "Соус вустерширский": [42, 43, 44],
    "Соус рыбный": [42, 43, 44],
    "Листья мяты": [18, 24, 25, 26],
    "Шафран": [42, 43, 44],
    "Лавровый лист": [42, 43, 44],
    "Корень имбиря": [42, 43, 44],
    "Клюквенный соус": [42, 43, 44],
    "Черный перец горошком": [42, 43, 44],
    "Сливочное масло": [42, 43, 44],
    "Зелень петрушки": [42, 43, 44],
    "Смесь перцев": [42, 43, 44],
    "Жгучий красный перец": [42, 43, 44],
    "Карри": [42, 43, 44],
    "Корица палочками": [42, 43, 44],
    "Анис звездчатый": [42, 43, 44],
    "Кардамон": [42, 43, 44],
    "Гвоздика": [42, 43, 44],
    "Мускатный орех целый": [42, 43, 44],
    "Тмин": [42, 43, 44],
    "Кориандр": [42, 43, 44]
}

# Генерация новых JSON-объектов
addition_id_counter = 1
new_addition_json_objects = []
for addition_name, product_ids in updated_additions.items():
    addition_object = {
        "Id": f"{addition_id_counter:03}1C9AB-9502-4687-B32D-9E6ACC752B1C",
        "Name": addition_name,
        "Price": random.randint(10, 500),
        "Count": random.randint(5, 20),
        "Portion": random.randint(1, 5),
        "Measure": "шт",
        "CurrencySymbol": "₽",
        "ProductIds": [f"{x:03}1C9AB-9502-4687-B32D-9E6ACC752B6C" for x in product_ids]
    }
    addition_id_counter += 1
    new_addition_json_objects.append(addition_object)
    
# Запись в файл
with open("MockAdditive.json", "w", encoding="utf-8") as file:
    json.dump(new_addition_json_objects, file, ensure_ascii=False, indent=4)