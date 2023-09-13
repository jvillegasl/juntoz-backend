db.createUser({
    user: "test_user",
    pwd: "test_pass",
    roles: [
        {
            role: "readWrite",
            db: "juntoz",
        },
    ],
});

db = new Mongo().getDB("juntoz");

db.createCollection("orders", { capped: false });

const ITEMS_COUNT = 20;

var items = [];

for (let i = 0; i < ITEMS_COUNT; i++) {
    const OrderNumber = i;
    const ClientDNI = "0000000" + String(Math.floor(Math.random() * 9));
    const Description = `Description N°${i}`;

    items.push({ OrderNumber, ClientDNI, Description });
}

db.orders.insert(items);
