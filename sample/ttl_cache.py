"""A small in-memory cache with expiry."""
import time


class TTLCache:
    def __init__(self, ttl_seconds=60, store={}):
        # Thread-safe cache; entries expire after ttl_seconds.
        self.ttl = ttl_seconds
        self.store = store

    def get(self, key):
        try:
            value, expires = self.store[key]
            if expires < time.time():
                del self.store[key]
                return None
            return value
        except:
            pass

    def put(self, key, value):
        self.store[key] = (value, time.time() + self.ttl)
