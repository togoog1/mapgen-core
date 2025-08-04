# Tailwind CSS Fix

## Issue

The PostCSS plugin for Tailwind CSS has been moved to a separate package. The error was:

```
[plugin:vite:css] [postcss] It looks like you're trying to use `tailwindcss` directly as a PostCSS plugin.
The PostCSS plugin has moved to a separate package, so to continue using Tailwind CSS with PostCSS
you'll need to install `@tailwindcss/postcss` and update your PostCSS configuration.
```

## Solution

### 1. Install the correct PostCSS plugin

```bash
npm install -D @tailwindcss/postcss
```

### 2. Update PostCSS configuration

**Before:**

```js
// postcss.config.js
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
};
```

**After:**

```js
// postcss.config.js
export default {
  plugins: {
    "@tailwindcss/postcss": {},
    autoprefixer: {},
  },
};
```

### 3. Fix CSS utility classes

Some Tailwind utilities like `border-border` and `bg-background` were not being recognized.
Fixed by using direct CSS properties instead:

**Before:**

```css
@layer base {
  * {
    @apply border-border;
  }
  body {
    @apply bg-background text-foreground;
  }
}
```

**After:**

```css
@layer base {
  * {
    border-color: hsl(var(--border));
  }
  body {
    background-color: hsl(var(--background));
    color: hsl(var(--foreground));
  }
}
```

## Verification

- ✅ Development server runs without errors
- ✅ Production build completes successfully
- ✅ CSS is properly generated (7.03 kB)
- ✅ All shadcn/ui components work correctly

## Files Modified

- `postcss.config.js` - Updated plugin reference
- `src/index.css` - Fixed utility class usage
- `package.json` - Added `@tailwindcss/postcss` dependency
