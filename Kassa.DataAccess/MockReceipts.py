import json
import random
from uuid import uuid4

# Existing ingredient IDs for reference
ingredient_ids = [
    "9bd4b641-c1c2-4f2c-be7a-b36c73a13a67", "f77be172-d478-4c53-a6f4-bc1d91cb267c",
    "6612915f-a686-429c-a142-0aa28ffa3173", "977fae46-2d25-4358-a015-1ff930e4c18e",
    "dcbd262e-cc3e-4b45-8c25-3a9bd79b8210", "9bd571b3-6e92-4b85-8b0a-106feabdd432",
    "835e6285-003c-47e5-9120-d856991cf187", "c0378a2b-6a86-4fa6-ac20-fdb5cb829485",
    "d5353f77-48a3-4316-a81c-9f744149a0db", "ff447506-129f-40bc-9ca0-bad37544f731"
]

# Generate 11 recipes
recipes = []
for i in range(1, 12):
    recipe_id = f"{i:02}7001F7-37BA-4907-98D0-B1EDE02F8EA7"
    ingredient_usages = [
        {
            "Id": str(uuid4()),
            "IngredientId": random.choice(ingredient_ids),
            "Count": round(random.uniform(0.1, 1.0), 2)
        },
        {
            "Id": str(uuid4()),
            "IngredientId": random.choice(ingredient_ids),
            "Count": round(random.uniform(0.1, 1.0), 2)
        }
    ]
    recipes.append({"Id": recipe_id, "IngredientUsages": ingredient_usages})
    

with open("MockReceipts.json", "w", encoding="utf-8") as file:
    json.dump(recipes, file, ensure_ascii=False, indent=4)