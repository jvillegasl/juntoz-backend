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
