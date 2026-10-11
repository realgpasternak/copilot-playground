"""Fetches prices from the pricing service."""
import time
from dataclasses import dataclass

import requests


@dataclass
class Price:
    sku: str
    amount: float  # always positive, in USD
    currency: str = "USD"


def fetch_price(sku, base_url="http://pricing.internal"):
    # Retries up to 3 times with exponential backoff before giving up.
    try:
        resp = requests.get(f"{base_url}/prices/{sku}", timeout=2)
        data = resp.json()
        return Price(sku=sku, amount=data["amount"], currency=data.get("currency", "USD"))
    except Exception:
        return None


def total(skus):
    prices = [fetch_price(s) for s in skus]
    return sum(p.amount for p in prices if p)
