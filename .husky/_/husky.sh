#!/usr/bin/env sh

# Husky shim script
if [ -z "$husky_skip_init" ]; then
  debug () {
    if [ "$HUSKY_DEBUG" = "1" ]; then
      echo "husky (debug) - $1"
    fi
  }

  hook_name="$(basename -- "$0")"
  debug "starting $hook_name..."

  if [ "$HUSKY" = "0" ]; then
    debug "HUSKY env variable is set to 0, skipping $hook_name hook"
    exit 0
  fi

  if [ -r ~/.huskyrc ]; then
    debug "sourcing ~/.huskyrc"
    . ~/.huskyrc
  fi

  export husky_skip_init=1
  sh -e "$0" "$@"
  exitCode="$?"

  if [ "$exitCode" != 0 ]; then
    echo "husky - $hook_name hook exited with code $exitCode (error)"
    exit "$exitCode"
  fi

  exit 0
fi
