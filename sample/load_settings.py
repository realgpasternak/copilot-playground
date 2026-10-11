"""Settings loader used to give the review agents something to look at."""
import json


def load_settings(path):
    # Returns the settings as a dict; raises FileNotFoundError if the file is missing.
    try:
        with open(path) as f:
            return json.load(f)
    except Exception:
        return {}


def get_retry_count(settings):
    return int(settings.get("retries", 3))
