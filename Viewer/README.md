# MapGen Viewer

A React + TypeScript + Vite playground for interactively testing and visualizing procedural map generation algorithms.

## Overview

The MapGen Viewer is a modern web application that provides a real-time interface for:

- **Testing map generators** with interactive parameter controls
- **Visualizing generated maps** with immediate feedback
- **Comparing different algorithms** side-by-side
- **Experimenting with parameters** using sliders and inputs

## Features

- 🎮 **Interactive playground** with real-time map generation
- 🎛️ **Dynamic parameter controls** automatically generated from generator metadata
- 🔄 **Hot reload** - changes reflect instantly without page refresh
- 📱 **Responsive design** - works on desktop and mobile
- 🚀 **Fast development** with Vite's lightning-fast HMR
- 🎨 **Modern UI** built with React 18 and TypeScript

## Tech Stack

- **React 19** - Modern UI library with concurrent features
- **TypeScript** - Type safety and excellent developer experience
- **Vite** - Ultra-fast build tool and dev server
- **React Router 7** - Client-side routing for SPA navigation
- **ESLint** - Code linting with React-specific rules

## Project Structure

```
Viewer/
├── src/
│   ├── components/     # Reusable UI components
│   ├── pages/         # Route components
│   ├── services/      # API communication with MapGen.Service
│   ├── types/         # TypeScript type definitions
│   ├── utils/         # Helper functions
│   └── App.tsx        # Main application component
├── public/            # Static assets
└── index.html         # Entry HTML file
```

## Getting Started

### Prerequisites

- Node.js 20 LTS or later
- npm (comes with Node.js)
- MapGen.Service running on port 5023

### Quick Start

```bash
# Navigate to Viewer directory
cd Viewer

# Install dependencies
npm install

# Start development server
npm run dev
```

The viewer will be available at `http://localhost:5173`

### Available Scripts

```bash
# Start development server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## Development Workflow

1. **Start the full stack** using the root startup script:

   ```bash
   # From the project root
   ./start-all.sh
   ```

2. **Edit React components** - changes reflect instantly in the browser

3. **Test generators** - the UI automatically fetches available generators from the Service

4. **Debug API calls** - use browser dev tools to inspect network requests

## API Integration

The Viewer communicates with MapGen.Service via REST API:

```typescript
// Example API calls
const generators = await fetch("http://localhost:5023/generators");
const map = await fetch("http://localhost:5023/generate/perlin", {
  method: "POST",
  body: JSON.stringify(parameters),
});
```

Key endpoints:

- `GET /generators` - List all available generators
- `POST /generate/{name}` - Generate a map using specific algorithm

## Components Architecture

### Core Components

- **`MapCanvas`** - Renders generated maps on HTML5 canvas
- **`ParameterPanel`** - Dynamic form controls for generator parameters
- **`GeneratorSelector`** - Dropdown for choosing map generation algorithms
- **`MapPreview`** - Real-time preview with loading states

### State Management

The app uses React's built-in state management:

- **Local state** for UI components
- **Context** for sharing data between components
- **Custom hooks** for API calls and data fetching

## Styling Guidelines

- Use CSS Modules or styled-components for component-specific styles
- Follow mobile-first responsive design principles
- Maintain consistent spacing and typography
- Use CSS variables for theming

## Performance Considerations

- **Image optimization** - maps are rendered as optimized PNGs
- **Debounced parameter changes** - prevents excessive API calls
- **Lazy loading** - components load on demand
- **Bundle splitting** - automatic code splitting with Vite

## Adding New Features

### New Generator Parameter Types

1. Add TypeScript types in `src/types/`
2. Create corresponding UI controls in `src/components/`
3. Update parameter parsing logic
4. Test with different generators

### New Visualization Modes

1. Extend the `MapCanvas` component
2. Add mode selection UI
3. Implement rendering logic for new modes
4. Update type definitions

## Environment Configuration

### Development

```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5023
VITE_ENABLE_DEBUG=true
```

### Production

```bash
# .env.production
VITE_API_BASE_URL=https://your-api-domain.com
VITE_ENABLE_DEBUG=false
```

## Browser Support

- Chrome 100+
- Firefox 100+
- Safari 15+
- Edge 100+

Modern browsers with ES2020 support required.

## Troubleshooting

### Common Issues

**Viewer can't connect to API:**

- Ensure MapGen.Service is running on port 5023
- Check CORS configuration in the Service
- Verify API base URL in environment variables

**Hot reload not working:**

- Restart the dev server
- Check for TypeScript errors in the console
- Clear browser cache

**Build fails:**

- Run `npm ci` to clean install dependencies
- Check for TypeScript errors: `npx tsc --noEmit`
- Ensure all imports are correctly typed

### Development Tips

- Use React DevTools browser extension for debugging
- Enable TypeScript strict mode for better type safety
- Use the Network tab to debug API communication
- Leverage Vite's fast refresh for optimal development speed

## Contributing

1. Follow the existing code style and patterns
2. Add TypeScript types for all new code
3. Test components in different screen sizes
4. Ensure API integration works with the Service layer

The Viewer is designed to be the fun, interactive face of MapGen - focus on great UX and smooth performance!
