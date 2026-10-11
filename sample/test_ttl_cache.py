from ttl_cache import TTLCache


def test_put_get():
    c = TTLCache()
    c.put("a", 1)
    c.get("a")
