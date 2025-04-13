#!/usr/bin/env python3
"""
Test script to verify Python execution from .NET
"""

import sys
import os
import json
from datetime import datetime

def main():
    """Test function to check if Python is working correctly"""
    # Print basic information
    print("Python version:", sys.version)
    print("Current working directory:", os.getcwd())
    
    # Output a simple JSON result that the C# code can parse
    result = {
        "success": True,
        "timestamp": datetime.now().isoformat(),
        "message": "Python execution is working correctly!",
        "python_version": sys.version,
        "working_directory": os.getcwd(),
        "argv": sys.argv
    }
    
    # Print to stdout as JSON for the C# code to capture
    print(json.dumps(result, indent=2))
    
    return 0

if __name__ == "__main__":
    sys.exit(main()) 